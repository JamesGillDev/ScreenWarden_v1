using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace ScreenWarden;

public static class TrayCaptureService
{
    // Small delay so the tray context menu can close and focus returns
    // to the window you actually want (important for ActiveWindowMonitor).
    private const int TrayDelayMs = 250;

    public static async Task CaptureFromTrayAsync(AppSettings settings, TaskbarIcon trayIcon)
    {
        try
        {
            await Task.Delay(TrayDelayMs);

            string path = ScreenCaptureService.CaptureToFile(settings);

            // Balloon tip (nice feedback without stealing focus)
            trayIcon.ShowBalloonTip(
                "GazeShot",
                $"Saved:\n{path}",
                BalloonIcon.Info);

            // Optional: also copy to clipboard (comment out if you don’t want it)
            // Clipboard.SetText(path);
        }
        catch (Exception ex)
        {
            trayIcon.ShowBalloonTip(
                "GazeShot Error",
                ex.Message,
                BalloonIcon.Error);
        }
    }
}
