# ScreenWarden

ScreenWarden is a modern, open-source Windows utility for automated and voice-activated screenshot capture. It features a system tray interface, customizable capture modes, and robust voice command management.

## Features
- Capture screenshots of the active window or the monitor under the mouse cursor
- System tray integration for quick access
- Voice command support (add, edit, delete, reset commands)
- Customizable save location
- Modern WPF UI with dark mode
- .NET 8 support

## Voice Command Sensitivity
ScreenWarden uses Windows Speech Recognition, which works best with clear, common English words and phrases. Some words (especially those with unusual sounds, slang, or strong consonants) may not be recognized reliably. For best results:
- Use simple, distinct phrases (e.g., "capture", "screenshot", "open settings").
- Avoid words that sound similar to each other or are difficult for speech engines (e.g., "pickle", profanity, or slang).
- Speak clearly and at a moderate pace.
- You can add, edit, or remove voice commands in the Settings window to find what works best for your voice and environment.

## Getting Started

### Prerequisites
- Windows 10 or later
- .NET 8 SDK

### Build & Run
1. Clone the repository:
   ```sh
   git clone https://github.com/JamesGillDev/ScreenWarden_v1
   cd ScreenWarden_v1.0
   ```
2. Open the solution in Visual Studio 2022 or later.
3. Build and run the project.

### Usage
- Use the tray icon to access capture and settings.
- Open Settings to manage capture mode, save location, and voice commands.
- Add, edit, or remove voice commands directly from the settings window.

## License
ScreenWarden is licensed under the Business Source License 1.1 (BSL-1.1). See [LICENSE](LICENSE) for details.

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## Author
James Gill
