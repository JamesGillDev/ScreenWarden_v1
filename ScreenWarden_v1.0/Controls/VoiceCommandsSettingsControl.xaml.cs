using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ScreenWarden.Models;
using ScreenWarden.Services;

namespace ScreenWarden.Controls
{
    public partial class VoiceCommandsSettingsControl : System.Windows.Controls.UserControl
    {
        private ObservableCollection<VoiceCommand> _commands = new ObservableCollection<VoiceCommand>();
        private VoiceCommandService? _voiceService;

        public VoiceCommandsSettingsControl()
        {
            InitializeComponent();
            CommandsListView.ItemsSource = _commands;
        }

        public void Initialize(VoiceCommandService voiceService)
        {
            _voiceService = voiceService;
            _commands.Clear();
            foreach (var cmd in voiceService.GetSettings().Commands)
                _commands.Add(cmd.Clone());
        }

        private void CommandsListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sel = CommandsListView.SelectedItem as VoiceCommand;
            
            // Update Edit button
            EditButton.IsEnabled = sel != null;
            if (sel != null)
            {
                EditButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(74, 74, 74));
                EditButton.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                EditButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(58, 58, 58));
                EditButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102));
            }
            
            // Update Delete button
            DeleteButton.IsEnabled = sel != null && !sel.IsBuiltIn;
            if (sel != null && !sel.IsBuiltIn)
            {
                DeleteButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(74, 74, 74));
                DeleteButton.Foreground = System.Windows.Media.Brushes.White;
            }
            else
            {
                DeleteButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(58, 58, 58));
                DeleteButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(102, 102, 102));
            }
        }

        private void CommandCheckBox_Changed(object sender, RoutedEventArgs e) { }

        private void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new VoiceCommandEditDialog { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true && dlg.Command != null)
            {
                dlg.Command.IsBuiltIn = false;
                _commands.Add(dlg.Command);
            }
        }

        private void EditCommand_Click(object sender, RoutedEventArgs e)
        {
            if (CommandsListView.SelectedItem is not VoiceCommand sel) return;
            var dlg = new VoiceCommandEditDialog(sel) { Owner = Window.GetWindow(this) };
            if (dlg.ShowDialog() == true && dlg.Command != null)
            {
                var idx = _commands.IndexOf(sel);
                if (idx >= 0) _commands[idx] = dlg.Command;
            }
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            if (CommandsListView.SelectedItem is VoiceCommand sel && !sel.IsBuiltIn)
                _commands.Remove(sel);
        }

        private void ResetDefaults_Click(object sender, RoutedEventArgs e)
        {
            _commands.Clear();
            foreach (var cmd in VoiceCommandsSettings.CreateDefault().Commands)
                _commands.Add(cmd);
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (_voiceService == null) return;
            _voiceService.UpdateSettings(new VoiceCommandsSettings
            {
                Commands = _commands.Select(c => c.Clone()).ToList()
            });
            
            // Custom message box with proper sizing
            var msgBox = new Window
            {
                Title = "Success",
                Width = 320,
                Height = 140,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 45))
            };
            var stack = new System.Windows.Controls.StackPanel { Margin = new Thickness(20), VerticalAlignment = System.Windows.VerticalAlignment.Center };
            stack.Children.Add(new System.Windows.Controls.TextBlock 
            { 
                Text = "Voice commands saved successfully!", 
                TextWrapping = TextWrapping.Wrap,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                TextAlignment = TextAlignment.Center
            });
            var btn = new System.Windows.Controls.Button 
            { 
                Content = "OK", 
                Width = 80, 
                Padding = new Thickness(5),
                Margin = new Thickness(0, 20, 0, 0), 
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center 
            };
            btn.Click += (s, args) => msgBox.Close();
            stack.Children.Add(btn);
            msgBox.Content = stack;
            msgBox.ShowDialog();
        }
    }

    public class BoolToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c) => (bool)value ? "Built-in" : "Custom";
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
