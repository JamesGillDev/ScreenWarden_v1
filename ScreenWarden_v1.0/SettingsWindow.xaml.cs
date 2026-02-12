using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using ScreenWarden.Models;

namespace ScreenWarden
{
    public partial class SettingsWindow : Window
    {
        private VoiceCommandService? _voiceService;
        private ObservableCollection<VoiceCommand> _voiceCommands = new();

        public SettingsWindow()
        {
            InitializeComponent();
            VoiceCommandsComboBox.ItemsSource = _voiceCommands;
            VoiceCommandsComboBox.DisplayMemberPath = "Phrase";
            VoiceCommandsComboBox.SelectionChanged += VoiceCommandsComboBox_SelectionChanged;
            AddVoiceCommandButton.Click += AddVoiceCommandButton_Click;
            EditVoiceCommandButton.Click += EditVoiceCommandButton_Click;
            DeleteVoiceCommandButton.Click += DeleteVoiceCommandButton_Click;
            ResetVoiceCommandsButton.Click += ResetVoiceCommandsButton_Click;
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
            }
        }

        private void DeleteVoiceCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (VoiceCommandsComboBox.SelectedItem is VoiceCommand sel && !sel.IsBuiltIn)
            {
                _voiceCommands.Remove(sel);
                SaveVoiceCommands();
            }
        }

        private void ResetVoiceCommandsButton_Click(object sender, RoutedEventArgs e)
        {
            _voiceCommands.Clear();
            foreach (var cmd in ScreenWarden.Services.VoiceCommandsSettings.CreateDefault().Commands)
                _voiceCommands.Add(cmd);
            SaveVoiceCommands();
        }

        private void SaveVoiceCommands()
        {
            if (_voiceService == null) return;
            _voiceService.UpdateSettings(new ScreenWarden.Services.VoiceCommandsSettings
            {
                Commands = _voiceCommands.Select(c => c.Clone()).ToList()
            });
        }
    }
}