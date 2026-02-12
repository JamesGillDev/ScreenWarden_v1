using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ScreenWarden.Models;

namespace ScreenWarden.Services
{
    public class VoiceCommandsSettings
    {
        private static readonly string SettingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScreenWarden");
        private static readonly string SettingsFile = Path.Combine(SettingsFolder, "voice_commands.json");

        public List<VoiceCommand> Commands { get; set; } = new List<VoiceCommand>();

        public static VoiceCommandsSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<VoiceCommandsSettings>(json);
                    if (settings != null && settings.Commands.Count > 0)
                        return settings;
                }
            }
            catch { }
            return CreateDefault();
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(SettingsFolder))
                    Directory.CreateDirectory(SettingsFolder);
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }

        public static VoiceCommandsSettings CreateDefault() => new VoiceCommandsSettings
        {
            Commands = new List<VoiceCommand>
            {
                new VoiceCommand("voice on screen warden", VoiceCommandAction.VoiceOn, true),
                new VoiceCommand("voice off screen warden", VoiceCommandAction.VoiceOff, true),
                new VoiceCommand("screenshot", VoiceCommandAction.Screenshot, true),
                new VoiceCommand("capture", VoiceCommandAction.Screenshot, true),
                new VoiceCommand("open settings", VoiceCommandAction.OpenSettings, true),
                new VoiceCommand("settings", VoiceCommandAction.OpenSettings, true),
                new VoiceCommand("exit screen warden", VoiceCommandAction.Exit, true),
            }
        };

        public static void Reset()
        {
            if (File.Exists(SettingsFile))
                File.Delete(SettingsFile);
        }

        public static void CreateDefaultFile()
        {
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);

            var settings = CreateDefault();
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }
    }
}
