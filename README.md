# ScreenWarden

ScreenWarden is a lightweight Windows utility for fast, distraction-free screenshots with optional voice commands and tray-first controls.

## Highlights

- Tray-based screenshot workflow
- Capture the active window or monitor under mouse cursor
- Optional voice-triggered commands
- Visual and audio capture feedback
- Editable voice command list in Settings
- Automatic file naming and save path support

## Repository Layout

- `ScreenWarden_v1.0/` - Main WPF application source
- `CreateRelease.ps1` - Packaging script for release ZIP output
- `ScreenWarden_v1.0_win-x64.zip` - Current packaged artifact at repository root

## Requirements

- Windows 10/11 (x64)
- .NET 8 Desktop Runtime (Windows x64) to run
- .NET 8 SDK to build from source

## Build

```powershell
dotnet build .\ScreenWarden_v1.0\ScreenWarden.csproj -c Release
```

## Run (Development)

```powershell
dotnet run --project .\ScreenWarden_v1.0\ScreenWarden.csproj
```

## Publish + Package for GitHub Release

From repository root:

```powershell
dotnet publish .\ScreenWarden_v1.0\ScreenWarden.csproj -c Release
.\CreateRelease.ps1
```

This produces:

- `ScreenWarden_v1.0\bin\Release\net8.0-windows10.0.19041.0\publish\` (publish output)
- `ScreenWarden_v1.0_win-x64.zip` (release-ready ZIP at root)

## Voice Command Notes

ScreenWarden uses Windows Speech Recognition. Accuracy is best with clear, distinct phrases such as:

- `capture`
- `take screenshot`
- `open settings`

If recognition is inconsistent, adjust phrases in Settings to shorter or less similar wording.

## Screenshots

### Settings - Active Window Monitor
<p align="center">
  <img src="https://github.com/user-attachments/assets/cd4b506e-1df2-4106-8b2d-faf9aaf82ac8" width="520" />
</p>

### Settings - Mouse Cursor Monitor
<p align="center">
  <img width="506" height="343" alt="settings-mouse" src="https://github.com/user-attachments/assets/139ebd98-7362-4f1e-909e-c580cd88e5ec" />
</p>

### System Tray Icon
<p align="center">
  <img width="234" height="74" alt="tray-icon" src="https://github.com/user-attachments/assets/a4decc30-9072-425f-b992-d8d83183593c" />
</p>

### Tray Menu
<p align="center">
  <img width="217" height="144" alt="tray-menu" src="https://github.com/user-attachments/assets/08e8265a-a097-415c-9c39-47a0c886b890" />
</p>

## License

Licensed under BSL-1.1. See `LICENSE`.
