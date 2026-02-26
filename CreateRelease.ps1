param(
    [string]$ProjectPath = ".\ScreenWarden_v1.0\ScreenWarden.csproj",
    [string]$Configuration = "Release",
    [string]$Framework = "net8.0-windows10.0.19041.0",
    [string]$Runtime = "win-x64",
    [string]$Version
)

# Script to create a GitHub release zip for ScreenWarden.
# Uses the csproj <Version> value unless -Version is provided.

if (-not (Test-Path $ProjectPath)) {
    Write-Host "ERROR: Project file not found at: $ProjectPath" -ForegroundColor Red
    exit 1
}

[xml]$projectXml = Get-Content -Path $ProjectPath
$projectVersion = $Version

if (-not $projectVersion) {
    $projectVersion = @(
        $projectXml.Project.PropertyGroup |
        ForEach-Object { $_.Version } |
        Where-Object { $_ -and $_.Trim() -ne "" }
    )[0]
}

if (-not $projectVersion) {
    Write-Host "ERROR: Could not determine version. Set <Version> in csproj or pass -Version." -ForegroundColor Red
    exit 1
}

$publishPath = ".\ScreenWarden_v1.0\bin\$Configuration\$Framework\publish"
$artifactDir = ".\release-artifacts"
$zipName = "ScreenWarden_v$projectVersion" + "_$Runtime.zip"
$zipOut = Join-Path $artifactDir $zipName

if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found at: $publishPath" -ForegroundColor Red
    Write-Host "Run this first from repository root:" -ForegroundColor Yellow
    Write-Host "  dotnet publish .\\ScreenWarden_v1.0\\ScreenWarden.csproj -c $Configuration" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $artifactDir)) {
    New-Item -ItemType Directory -Path $artifactDir | Out-Null
}

if (Test-Path $zipOut) {
    Remove-Item $zipOut -Force
}

Compress-Archive -Path "$publishPath\*" -DestinationPath $zipOut -Force

if (Test-Path $zipOut) {
    Write-Host "SUCCESS: Created $(Resolve-Path $zipOut)" -ForegroundColor Green
    Write-Host "Version: $projectVersion"
    Write-Host "Size: $([math]::Round((Get-Item $zipOut).Length / 1MB, 2)) MB"
} else {
    Write-Host "ERROR: Failed to create zip" -ForegroundColor Red
}
