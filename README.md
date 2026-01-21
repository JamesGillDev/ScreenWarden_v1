# ScreenWarden

A lightweight Windows desktop utility for fast, distraction-free screenshots with optional voice control and system tray operation.

ScreenWarden was built to support focused workflows where stopping to manage UI or file dialogs breaks concentration. 
The app stays resident in the system tray, provides clear feedback, and allows screenshots to be captured instantly — including via optional voice commands.

## Features

- One-click screenshot capture
- Optional voice-command activation
- Runs quietly in the Windows system tray
- Visual and audio feedback on capture
- Automatic file naming
- Designed for fast, repeatable use

## How It Works

ScreenWarden is a WPF desktop application built on .NET.  
It initializes as a background process with a tray icon and listens for user input via UI interaction or optional voice commands.

Screenshot capture is handled by a dedicated service layer, separating UI concerns from system interaction. 
This structure allows future expansion such as region capture, hotkeys, or multi-monitor support.

## Screenshots

### Settings
<img width="506" height="343" alt="settings-active" src="https://github.com/user-attachments/assets/cd4b506e-1df2-4106-8b2d-faf9aaf82ac8" />


![Settings Window](\docs\screenshots\settings-mouse.png)

### Tray Menu / Tray Icon
![Tray Menu](\docs\screenshots\tray-menu.png)
![Tray Menu](\docs\screenshots\tray-icon.png)

## Download

A prebuilt Windows executable is available on the Releases page:

➡ Download ScreenWarden for Windows:  
https://github.com/JamesGillDev/ScreenWarden_v1/releases

## Tech Stack

- C#
- .NET 8
- WPF
- Windows Desktop APIs

## Status

This project is actively developed. Planned enhancements include:
- Global hotkey support
- Region-based capture
- Multi-monitor awareness
- Optional image preview overlay

## License

This project is licensed under the MIT License.
