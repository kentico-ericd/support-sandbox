<#
.Synopsis
    Creates a deployment package for uploading to the Xperience Cloud environment.
#>
[CmdletBinding()]
param (
    # Output path for exported deployment package.
    [Parameter(Mandatory = $false)]
    [string]$OutputPackagePath = "../DeploymentPackage.zip",

    # The name of the main web application assembly used as the starting point by the Xperience Cloud.
    [Parameter(Mandatory = $false)]
    [string]$AssemblyName = "Sandbox",

    # If present, the custom build number won't be used as a "Product version" suffix in the format yyyyMMddHHmm.
    [switch]$KeepProductVersion,

    # Mode in which the storage assets are deployed, if present.
    [ValidateSet("Create", "CreateUpdate")]
    [String]$StorageAssetsDeploymentMode = "Create",

    # If present, indicates the deployment package code supports zero downtime deployment.
    [switch]$ZeroDowntimeSupportEnabled
)
$ErrorActionPreference = "Stop"

$CDRepositoryFolderName = "`$CDRepository"
$StorageAssetsFolderName = "`$StorageAssets"
$BuildNumber = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmm")

# Resolve full paths
$WebProjectFullPath = Resolve-Path "../src"
$OutputFolderFullPath = Join-Path $WebProjectFullPath "/bin/CloudDeployment/"
$MetadataFileFullPath = Join-Path $OutputFolderFullPath "cloud-metadata.json"
$LocalCDRepositoryFullPath = Join-Path $WebProjectFullPath $CDRepositoryFolderName
$AssemblyFullPath = Join-Path $OutputFolderFullPath "$AssemblyName.dll" -Resolve

# Storage assets paths
$LocalStorageAssetsFullPath = Join-Path $WebProjectFullPath $StorageAssetsFolderName
$OutputStorageAssetsFullPath = Join-Path $OutputFolderFullPath $StorageAssetsFolderName

# Check for non-existing or empty CD repository which could corrupt the database
if (-not (Test-Path $LocalCDRepositoryFullPath) -or (@(Get-ChildItem -Path $LocalCDRepositoryFullPath -Directory).Count -le 0)) {
    throw "Cannot detect CD repository on path '$LocalCDRepositoryFullPath'. Make sure to run 'dotnet run --kxp-cd-store --repository-path ""```$CDRepository""' before 'Export-DeploymentPackage.ps1'."
}

# Remove previously published website
Remove-Item -Recurse -Force $OutputFolderFullPath -ErrorAction SilentlyContinue

# Publish the application in the 'Release' mode
$PublishCommand = "dotnet publish $WebProjectFullPath --nologo -c Release -o $OutputFolderFullPath"

if (!$KeepProductVersion) {
    $PublishCommand += " --version-suffix $BuildNumber"
}

Invoke-Expression $PublishCommand

if ($LASTEXITCODE -ne 0) {
    throw "Publishing the website failed."
}

# Copy content of the CD repository to the output folder
Copy-Item -Force -Recurse "$LocalCDRepositoryFullPath" -Destination $OutputFolderFullPath

if (Test-Path $LocalStorageAssetsFullPath) {
    # Check if storage asset top-level directories have valid names
    Get-ChildItem -Path $LocalStorageAssetsFullPath | % {
        if ($_.Name -cnotmatch "^[a-z0-9](?!.*--)[a-z0-9-]{1,61}[a-z0-9]$") {
            throw "Storage asset directory '$($_.FullName)' does not have a valid name. Top level storage asset directories must have names that are 3-63 characters long and contain only lowercase letters, numbers or dashes (-). Every dash symbol must be surrounded by letters or numbers."
        }
    }

    # Copy storage assets to the output folder
    New-Item -Force -ItemType Directory $OutputStorageAssetsFullPath | Out-Null
    Copy-Item -Force -Recurse "$LocalStorageAssetsFullPath/*" -Destination $OutputStorageAssetsFullPath

    # Deployed assets need to have lowercase names
    Get-ChildItem -Path $OutputStorageAssetsFullPath -Recurse | % {
        $lowercasedAssetName = $_.Name.ToLowerInvariant()

        if ($_.Name -cne $lowercasedAssetName) {
            Rename-Item -Force $_.FullName "$($_.Name).tmp"
            Rename-Item -Force "$($_.FullName).tmp" $lowercasedAssetName
        }
    }
}

$PackageMetadata = @{
    AssemblyName = $AssemblyName
    Version      = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($AssemblyFullPath).ProductVersion
    SupportsZeroDowntimeDeployment = $ZeroDowntimeSupportEnabled.IsPresent
}

# Add necessary metadata if storage assets folder has been exported as well
if (Test-Path $OutputStorageAssetsFullPath) {
    $PackageMetadata.Add("StorageAssetsDirectory", $StorageAssetsFolderName)
    $PackageMetadata.Add("StorageAssetsDeploymentMode", $StorageAssetsDeploymentMode)
}

# Create all necessary metadata for cloud-based package deployment
$PackageMetadata | ConvertTo-Json -Depth 2 | Set-Content $MetadataFileFullPath -Encoding utf8

# Create a deployment package
if (Test-Path -Path $OutputPackagePath -PathType Container) {
    $OutputPackagePath = Join-Path -Path $OutputPackagePath -ChildPath "./DeploymentPackage.zip"
}
Compress-Archive -Force -Path "$OutputFolderFullPath/*" -DestinationPath $OutputPackagePath