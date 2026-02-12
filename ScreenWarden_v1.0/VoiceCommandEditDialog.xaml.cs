using System.Windows;
using ScreenWarden.Models;

namespace ScreenWarden
{
    public partial class VoiceCommandEditDialog : Window
    {
        public VoiceCommand? Command { get; private set; }

        public VoiceCommandEditDialog()
        {
            InitializeComponent();
            ActionComboBox.ItemsSource = System.Enum.GetValues(typeof(VoiceCommandAction));
            EnabledCheckBox.IsChecked = true;
        }

        public VoiceCommandEditDialog(VoiceCommand command) : this()
        {
            PhraseTextBox.Text = command.Phrase;
            ActionComboBox.SelectedItem = command.Action;
            EnabledCheckBox.IsChecked = command.IsEnabled;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhraseTextBox.Text) || ActionComboBox.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please enter a phrase and select an action.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Command = new VoiceCommand
            {
                Phrase = PhraseTextBox.Text.Trim(),
                Action = (VoiceCommandAction)ActionComboBox.SelectedItem,
                IsEnabled = EnabledCheckBox.IsChecked == true
            };
            DialogResult = true;
        }
    }
}
