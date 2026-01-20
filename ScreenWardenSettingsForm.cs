ScreenWardenSettingsForm();
{
    InitializeComponent();
    // Set modeComboBox colors after initialization
    modeComboBox.ForeColor = System.Drawing.Color.White; // Use Black if background is light
    modeComboBox.BackColor = System.Drawing.Color.Black; // Use White if background is dark
    modeComboBox.Refresh(); // Optional: force redraw
}