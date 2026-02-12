using System;
using System.Linq;
using System.Windows;
using ScreenWarden.Models;

namespace ScreenWarden.Controls
{
    public partial class VoiceCommandEditDialog : Window
    {
        public VoiceCommand? Command { get; private set; }
        private readonly VoiceCommand? _existing;

        public VoiceCommandEditDialog(VoiceCommand? existing = null)
        {
            InitializeComponent();
            _existing = existing;

            ActionComboBox.ItemsSource = Enum.GetValues(typeof(VoiceCommandAction))
                .Cast<VoiceCommandAction>()
                .Select(a => new { Value = a, Display = a.ToString() }).ToList();
            ActionComboBox.DisplayMemberPath = "Display";
            ActionComboBox.SelectedValuePath = "Value";
            ActionComboBox.SelectedIndex = 0;

            if (_existing != null)
            {
                Title = "Edit Command";
                PhraseTextBox.Text = _existing.Phrase;
                ActionComboBox.SelectedValue = _existing.Action;
                EnabledCheckBox.IsChecked = _existing.IsEnabled;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var phrase = PhraseTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(phrase)) { System.Windows.MessageBox.Show("Enter a phrase."); return; }

            Command = new VoiceCommand
            {
                Id = _existing?.Id ?? Guid.NewGuid().ToString(),
                Phrase = phrase.ToLowerInvariant(),
                Action = (VoiceCommandAction)ActionComboBox.SelectedValue,
                IsEnabled = EnabledCheckBox.IsChecked ?? true,
                IsBuiltIn = _existing?.IsBuiltIn ?? false
            };
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
