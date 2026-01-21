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
## Screenshots

### Settings – Active Window Monitor
<p align="center">
  <img src="https://github.com/user-attachments/assets/cd4b506e-1df2-4106-8b2d-faf9aaf82ac8" width="520" />
</p>

Captures the currently active window for precise, focused screenshots.

---

### Settings – Mouse Cursor Monitor
<p align="center">
  <img src="https://github.com/user-attachments/assets/fa91d447-0929-44d3-a2e6-254e10db0ade" width="520" />
</p>

Tracks the monitor containing the mouse cursor and captures that display.

---

### System Tray Icon
<p align="center">
  <img width="234" height="74" alt="tray-icon" src="https://github.com/user-attachments/assets/8fb0d349-a611-44b2-a2f8-dd23a86919c8" />
</p>

Always-on tray presence for quick access without interrupting workflow.

---

### Tray Menu
<p align="center">
  <img src="https://github.com/user-attachments/assets/930ea180-6380-4e74-b96e-ca8c6361cfd2" width="300" />
</p>

Quick actions for capture, mode switching, settings, and exit.

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
