using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ScreenWarden;

public partial class App : System.Windows.Application
{
    public AppSettings Settings { get; } = new();

    private TaskbarIcon? _trayIcon;
    private MainWindow? _mainWindow;

    // Tray menu items
    private MenuItem? _miCapture;
    private MenuItem? _miModeActive;
    private MenuItem? _miModeMouse;
    private MenuItem? _miOpenSettings;
    private MenuItem? _miExit;
    private MenuItem? _miVoiceToggle;

    private VoiceCommandService? _voice;
    private bool _voiceEventsWired;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

        // Create settings window but keep it hidden (tray app stays alive)
        _mainWindow = new MainWindow(Settings);
        _mainWindow.Hide();

        WireTrayMenuHandlers();
        UpdateTrayModeChecks();

        StartVoiceService();
        UpdateVoiceMenuUI();
    }

    private void WireTrayMenuHandlers()
    {
        if (_trayIcon?.ContextMenu == null) return;

        var menu = _trayIcon.ContextMenu;

        _miCapture = FindMenuItemByHeader(menu, "Capture Now");
        _miOpenSettings = FindMenuItemByHeader(menu, "Open Settings");
        _miExit = FindMenuItemByHeader(menu, "Exit");

        _miModeActive = FindMenuItemByHeader(menu, "Active Window Monitor");
        _miModeMouse = FindMenuItemByHeader(menu, "Mouse Cursor Monitor");

        // Optional tray toggle item (Header must match your App.xaml menu)
        _miVoiceToggle = FindMenuItemByHeader(menu, "Voice: On/Off");

        if (_miCapture != null)
        {
            _miCapture.Click -= Tray_CaptureNow_Click;
            _miCapture.Click += Tray_CaptureNow_Click;
        }

        if (_miOpenSettings != null)
        {
            _miOpenSettings.Click -= Tray_OpenSettings_Click;
            _miOpenSettings.Click += Tray_OpenSettings_Click;
        }

        if (_miExit != null)
        {
            _miExit.Click -= Tray_Exit_Click;
            _miExit.Click += Tray_Exit_Click;
        }

        if (_miModeActive != null)
        {
            _miModeActive.Click -= Tray_Mode_Active_Click;
            _miModeActive.Click += Tray_Mode_Active_Click;
        }

        if (_miModeMouse != null)
        {
            _miModeMouse.Click -= Tray_Mode_Mouse_Click;
            _miModeMouse.Click += Tray_Mode_Mouse_Click;
        }

        if (_miVoiceToggle != null)
        {
            _miVoiceToggle.IsCheckable = true;
            _miVoiceToggle.Click -= Tray_VoiceToggle_Click;
            _miVoiceToggle.Click += Tray_VoiceToggle_Click;
        }

        menu.Opened -= TrayMenu_Opened;
        menu.Opened += TrayMenu_Opened;
    }

    private void TrayMenu_Opened(object? sender, RoutedEventArgs e)
    {
        UpdateTrayModeChecks();
        UpdateVoiceMenuUI();
    }

    private static MenuItem? FindMenuItemByHeader(ItemsControl parent, string headerText)
    {
        foreach (var item in parent.Items)
        {
            if (item is MenuItem mi)
            {
                if (mi.Header?.ToString() == headerText)
                    return mi;

                var found = FindMenuItemByHeader(mi, headerText);
                if (found != null)
                    return found;
            }
        }
        return null;
    }

    private void UpdateTrayModeChecks()
    {
        if (_miModeActive == null || _miModeMouse == null) return;

        _miModeActive.IsChecked = Settings.Mode == CaptureMode.ActiveWindowMonitor;
        _miModeMouse.IsChecked = Settings.Mode == CaptureMode.MouseCursorMonitor;
    }

    private void UpdateVoiceMenuUI()
    {
        if (_miVoiceToggle == null || _voice == null) return;
        _miVoiceToggle.IsChecked = _voice.IsEnabled;
    }

    private async void Tray_CaptureNow_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_trayIcon == null) return;
            await TrayCaptureService.CaptureFromTrayAsync(Settings, _trayIcon);
        }
        catch (Exception ex)
        {
            _trayIcon?.ShowBalloonTip("ScreenWarden", $"Capture failed: {ex.Message}", BalloonIcon.Error);
        }
    }

    private void Tray_Mode_Active_Click(object sender, RoutedEventArgs e)
    {
        Settings.Mode = CaptureMode.ActiveWindowMonitor;
        UpdateTrayModeChecks();
        _mainWindow?.SyncFromSettings();
        _trayIcon?.ShowBalloonTip("ScreenWarden", "Mode: Active Window Monitor", BalloonIcon.Info);
    }

    private void Tray_Mode_Mouse_Click(object sender, RoutedEventArgs e)
    {
        Settings.Mode = CaptureMode.MouseCursorMonitor;
        UpdateTrayModeChecks();
        _mainWindow?.SyncFromSettings();
        _trayIcon?.ShowBalloonTip("ScreenWarden", "Mode: Mouse Cursor Monitor", BalloonIcon.Info);
    }

    private void Tray_OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            if (_mainWindow == null) return;

            if (!_mainWindow.IsVisible)
                _mainWindow.Show();

            _mainWindow.SyncFromSettings();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
            _mainWindow.Topmost = true;
            _mainWindow.Topmost = false;
            _mainWindow.Focus();
        });
    }

    private void Tray_VoiceToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_voice == null) return;

        _voice.Toggle();
        UpdateVoiceMenuUI();

        _trayIcon?.ShowBalloonTip(
            "ScreenWarden",
            _voice.IsEnabled ? "Voice commands ON" : "Voice commands OFF",
            BalloonIcon.Info);
    }

    private void StartVoiceService()
    {
        _voice ??= new VoiceCommandService();

        if (!_voiceEventsWired)
        {
            _voice.ScreenshotRequested += Voice_ScreenshotRequested;
            _voice.OpenSettingsRequested += Voice_OpenSettingsRequested;
            _voice.ExitRequested += Voice_ExitRequested;

            // Tray balloons when voice state changes via voice commands too
            _voice.VoiceStateChanged += enabled =>
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateVoiceMenuUI();
                    _trayIcon?.ShowBalloonTip(
                        "ScreenWarden",
                        enabled ? "Voice commands ON" : "Voice commands OFF",
                        BalloonIcon.Info);
                });
            };

            _voiceEventsWired = true;
        }

        try
        {
            _voice.Start();
            UpdateVoiceMenuUI();
        }
        catch (Exception ex)
        {
            _trayIcon?.ShowBalloonTip("ScreenWarden", $"Voice start failed: {ex.Message}", BalloonIcon.Error);
        }
    }

    private void Voice_ScreenshotRequested()
    {
        Dispatcher.Invoke(() =>
        {
            Tray_CaptureNow_Click(this, new RoutedEventArgs());
        });
    }

    private void Voice_OpenSettingsRequested()
    {
        Dispatcher.Invoke(() =>
        {
            Tray_OpenSettings_Click(this, new RoutedEventArgs());
        });
    }

    private void Voice_ExitRequested()
    {
        Dispatcher.Invoke(() =>
        {
            Tray_Exit_Click(this, new RoutedEventArgs());
        });
    }

    private void Tray_Exit_Click(object sender, RoutedEventArgs e)
    {
        _voice?.Dispose();
        _trayIcon?.Dispose();
        Current.Shutdown();
    }

    // Add this method to the App class to fix CS1061
    public void SetMode(CaptureMode mode)
    {
        if (Settings != null)
        {
            Settings.Mode = mode;
            // If you need to update tray UI or other components, call those methods here.
            UpdateTrayModeChecks();
        }
    }
}
