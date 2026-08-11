$webProjectDir = Resolve-Path("../src")
$cdRepoDir = Resolve-Path(Join-Path $webProjectDir "`$CDRepository")
& dotnet run `
    --no-build `
    --no-restore `
    --project $webProjectDir `
    -- `
    --kxp-cd-store `
    --repository-path $cdRepoDir