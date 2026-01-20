using System;
using System.IO;

namespace ScreenWarden;

public class AppSettings
{
    public CaptureMode Mode { get; set; } = CaptureMode.MouseCursorMonitor;

    public string SaveFolder { get; set; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "Screenshots");

    public int CaptureDelayMs { get; set; } = 1000; // Provide a default value to fix uninitialized property

    public string FontFamily { get; set; } = "Segoe UI";
}
