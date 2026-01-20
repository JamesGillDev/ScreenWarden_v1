using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;
using System.Windows.Media;

namespace ScreenWarden;

public partial class MainWindow : Window
{
    private readonly AppSettings _settings;

    public MainWindow(AppSettings settings)
    {
        InitializeComponent();

        _settings = settings;

        // Populate mode dropdown
        ModeCombo.ItemsSource = Enum.GetValues(typeof(CaptureMode));
        SyncFromSettings();
    }

    public void SyncFromSettings()
    {
        ModeCombo.SelectedItem = _settings.Mode;
        FolderText.Text = _settings.SaveFolder;
    }

    private void ModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModeCombo.SelectedItem is not CaptureMode mode) return;

        // Fix: Get the current App instance via Application.Current as App
        if (System.Windows.Application.Current is not App app)
        {
            _settings.Mode = mode; // fallback
        }
        else
        {
            app.SetMode(mode);
        }
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        // Keep it simple: let user pick a folder using Win32 dialog fallback (file dialog trick)
        // If you already use FolderBrowserDialog elsewhere, use that instead.
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Select this folder"
        };

        if (dlg.ShowDialog(this) == true)
        {
            var folder = Path.GetDirectoryName(dlg.FileName);
            if (!string.IsNullOrWhiteSpace(folder))
            {
                _settings.SaveFolder = folder;
                FolderText.Text = folder;

                StatusText.Text = $"Save folder set to: {folder}";
            }
        }
    }

    private void CaptureNow_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            StatusText.Text = $"Capturing in {_settings.Mode}...";

            // Optional delay (if you have CaptureDelayMs in settings)
            // If you don’t, it’ll just capture immediately.
            int delayMs = 0;
            try { delayMs = _settings.CaptureDelayMs; } catch { /* ignore */ }

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(delayMs) };
            timer.Tick += (_, __) =>
            {
                timer.Stop();
                try
                {
                    string path = ScreenCaptureService.CaptureToFile(_settings);
                    StatusText.Text = $"Saved: {path}";
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Error: {ex.Message}";
                }
            };

            timer.Start();
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        // Tray app should stay alive — hide window instead of closing.
        e.Cancel = true;
        Hide();
    }
}
