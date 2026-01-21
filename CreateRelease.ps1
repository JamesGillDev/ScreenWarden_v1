# Script to create a GitHub release zip for ScreenWarden
# Builds and packages ScreenWarden for GitHub Releases
# Framework-dependent (requires .NET 8 Desktop Runtime)
# Outputs ZIP to /release-artifacts

# Framework-dependent publish path (no RID)
$publishPath = ".\ScreenWarden_v1.0\bin\Release\net8.0-windows10.0.19041.0\publish"
$zipOut = ".\ScreenWarden_v1.0_win-x64.zip"

# Verify publish folder exists
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found at: $publishPath" -ForegroundColor Red
    Write-Host "Run this first from ScreenWarden_v1.0 subfolder:" -ForegroundColor Yellow
    Write-Host "  dotnet publish -c Release -r win-x64 --self-contained" -ForegroundColor Yellow
    exit 1
}

# Remove existing zip if present
if (Test-Path $zipOut) { Remove-Item $zipOut -Force }

# Create the zip
Compress-Archive -Path "$publishPath\*" -DestinationPath $zipOut -Force

if (Test-Path $zipOut) {
    Write-Host "SUCCESS: Created $(Resolve-Path $zipOut)" -ForegroundColor Green
    Write-Host "Size: $([math]::Round((Get-Item $zipOut).Length / 1MB, 2)) MB"
} else {
    Write-Host "ERROR: Failed to create zip" -ForegroundColor Red
}
