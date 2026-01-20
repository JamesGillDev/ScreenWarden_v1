using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenWarden;

public static class ScreenCaptureService
{
    // --- WIN32: Active window handle + bounds ---
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
        
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left; public int Top; public int Right; public int Bottom;
    }

    public static string CaptureToFile(AppSettings settings)
    {
        Screen screen = settings.Mode switch
        {
            CaptureMode.ActiveWindowMonitor => GetScreenFromActiveWindow() ?? Screen.PrimaryScreen!,
            CaptureMode.MouseCursorMonitor => GetScreenFromMouseCursor() ?? Screen.PrimaryScreen!,
            _ => Screen.PrimaryScreen!
        };

        Directory.CreateDirectory(settings.SaveFolder);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string safeDevice = screen.DeviceName.Replace("\\", "").Replace(".", "");
        string fileName = $"Screenshot_{timestamp}_{safeDevice}.png";
        string fullPath = Path.Combine(settings.SaveFolder, fileName);

        CaptureScreenToPng(screen, fullPath);

        return fullPath;
    }

    private static Screen? GetScreenFromMouseCursor()
    {
        var p = Cursor.Position;
        return Screen.FromPoint(p);
    }

    private static Screen? GetScreenFromActiveWindow()
    {
        IntPtr hwnd = GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return null;

        if (!GetWindowRect(hwnd, out var rect)) return null;

        // Use center point of active window to pick monitor
        int centerX = rect.Left + ((rect.Right - rect.Left) / 2);
        int centerY = rect.Top + ((rect.Bottom - rect.Top) / 2);

        return Screen.FromPoint(new System.Drawing.Point(centerX, centerY));
    }

    private static void CaptureScreenToPng(Screen screen, string filePath)
    {
        var bounds = screen.Bounds;

        using var bmp = new Bitmap(bounds.Width, bounds.Height);
        using var g = Graphics.FromImage(bmp);

        g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);

        bmp.Save(filePath, ImageFormat.Png);
    }
}
