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

### Settings Window
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/settings-main.png" width="680" alt="ScreenWarden settings window" />
</p>

### Voice Commands Dropdown
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/settings-voice-commands-dropdown.png" width="680" alt="Voice commands dropdown in settings" />
</p>

### Capture Mode Dropdown
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/settings-mode-dropdown.png" width="680" alt="Capture mode dropdown in settings" />
</p>

### Edit Voice Command (Empty)
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/voice-command-edit-empty.png" width="420" alt="Empty edit voice command dialog" />
</p>

### Edit Voice Command (Action List)
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/voice-command-edit-action-dropdown.png" width="420" alt="Edit voice command dialog with action list open" />
</p>

### Edit Voice Command (Configured)
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/voice-command-edit-filled.png" width="420" alt="Configured edit voice command dialog" />
</p>

### Tray Menu
<p align="center">
  <img src="ScreenWarden_v1.0/docs/screenshots/tray-menu-context.png" width="220" alt="ScreenWarden tray context menu" />
</p>

## License

Licensed under BSL-1.1. See `LICENSE`.
