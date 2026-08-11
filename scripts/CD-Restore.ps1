$webProjectDir = Resolve-Path("../src")
$cdRepoDir = Resolve-Path(Join-Path $webProjectDir "CDRepository")
$command = "dotnet run " + `
    "--no-build " + `
    "--no-restore " + `
    "--project $webProjectDir " + `
    "-- --kxp-cd-restore " + `
    "--repository-path `"$cdRepoDir`" "

Invoke-Expression -Command $command