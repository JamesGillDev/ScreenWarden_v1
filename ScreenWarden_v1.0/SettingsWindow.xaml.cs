using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using ScreenWarden.Models;
using System;
using System.Windows.Forms;

namespace ScreenWarden
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _appSettings;
        private VoiceCommandService? _voiceService;
        private ObservableCollection<VoiceCommand> _voiceCommands = new();

        public SettingsWindow(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            InitializeComponent();
            VoiceCommandsComboBox.ItemsSource = _voiceCommands;
            VoiceCommandsComboBox.DisplayMemberPath = "Phrase";
            VoiceCommandsComboBox.SelectionChanged += VoiceCommandsComboBox_SelectionChanged;
            AddVoiceCommandButton.Click += AddVoiceCommandButton_Click;
            EditVoiceCommandButton.Click += EditVoiceCommandButton_Click;
            DeleteVoiceCommandButton.Click += DeleteVoiceCommandButton_Click;
            ResetVoiceCommandsButton.Click += ResetVoiceCommandsButton_Click;
            ModeComboBox.ItemsSource = Enum.GetValues(typeof(CaptureMode));
            ModeComboBox.SelectionChanged += ModeComboBox_SelectionChanged;
            BrowseButton.Click += BrowseButton_Click;
            CaptureNowButton.Click += CaptureNowButton_Click;
            SyncFromSettings();
        }

        public void SyncFromSettings()
        {
            ModeComboBox.SelectedItem = _appSettings.Mode;
            SavePathTextBox.Text = _appSettings.SaveFolder;
        }

        public void SetVoiceService(VoiceCommandService voiceService)
        {
            _voiceService = voiceService;
            _voiceCommands.Clear();
            foreach (var cmd in voiceService.GetSettings().Commands)
                _voiceCommands.Add(cmd.Clone());
        }

        private void VoiceCommandsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EditVoiceCommandButton.IsEnabled = VoiceCommandsComboBox.SelectedItem != null;
            DeleteVoiceCommandButton.IsEnabled = VoiceCommandsComboBox.SelectedItem is VoiceCommand cmd && !cmd.IsBuiltIn;
        }

        private void AddVoiceCommandButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VoiceCommandEditDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.Command != null)
            {
                dlg.Command.IsBuiltIn = false;
                _voiceCommands.Add(dlg.Command);
                SaveVoiceCommands();
                VoiceCommandsComboBox.Items.Refresh();
            }
        }

        private void EditVoiceCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (VoiceCommandsComboBox.SelectedItem is not VoiceCommand sel) return;
            var dlg = new VoiceCommandEditDialog(sel) { Owner = this };
            if (dlg.ShowDialog() == true && dlg.Command != null)
            {
                var idx = _voiceCommands.IndexOf(sel);
                if (idx >= 0) _voiceCommands[idx] = dlg.Command;
                SaveVoiceCommands();
                VoiceCommandsComboBox.Items.Refresh();
            }
        }

        private void DeleteVoiceCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (VoiceCommandsComboBox.SelectedItem is VoiceCommand sel && !sel.IsBuiltIn)
            {
                _voiceCommands.Remove(sel);
                SaveVoiceCommands();
                VoiceCommandsComboBox.Items.Refresh();
            }
        }

        private void ResetVoiceCommandsButton_Click(object sender, RoutedEventArgs e)
        {
            _voiceCommands.Clear();
            foreach (var cmd in ScreenWarden.Services.VoiceCommandsSettings.CreateDefault().Commands)
                _voiceCommands.Add(cmd);
            SaveVoiceCommands();
            VoiceCommandsComboBox.Items.Refresh();
        }

        private void SaveVoiceCommands()
        {
            if (_voiceService == null) return;
            _voiceService.UpdateSettings(new ScreenWarden.Services.VoiceCommandsSettings
            {
                Commands = _voiceCommands.Select(c => c.Clone()).ToList()
            });
        }

        private void ModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ModeComboBox.SelectedItem is CaptureMode mode)
            {
                _appSettings.Mode = mode;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SavePathTextBox.Text = dialog.SelectedPath;
                _appSettings.SaveFolder = dialog.SelectedPath;
            }
        }

        private void CaptureNowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModeComboBox.SelectedItem is CaptureMode mode)
                {
                    _appSettings.Mode = mode;
                }

                if (!string.IsNullOrWhiteSpace(SavePathTextBox.Text))
                {
                    _appSettings.SaveFolder = SavePathTextBox.Text;
                }

                var path = ScreenCaptureService.CaptureToFile(_appSettings);
                System.Windows.MessageBox.Show($"Screenshot saved: {path}", "Capture Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Capture failed: {ex.Message}", "Capture Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
