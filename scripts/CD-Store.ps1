$webProjectDir = Resolve-Path("../src")
$cdRepoDir = Resolve-Path(Join-Path $webProjectDir "CDRepository")
$command = "dotnet run " + `
    "--no-build " + `
    "--no-restore " + `
    "--project $webProjectDir " + `
    "-- --kxp-cd-store " + `
    "--repository-path `"$cdRepoDir`" "

Invoke-Expression -Command $command