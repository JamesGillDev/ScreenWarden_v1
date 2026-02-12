using System;

namespace ScreenWarden.Models
{
    public class VoiceCommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Phrase { get; set; } = string.Empty;
        public VoiceCommandAction Action { get; set; } = VoiceCommandAction.Screenshot;
        public bool IsEnabled { get; set; } = true;
        public bool IsBuiltIn { get; set; } = false;

        public VoiceCommand() { }

        public VoiceCommand(string phrase, VoiceCommandAction action, bool isBuiltIn = false)
        {
            Phrase = phrase;
            Action = action;
            IsBuiltIn = isBuiltIn;
            IsEnabled = true;
        }

        public VoiceCommand Clone() => new VoiceCommand
        {
            Id = this.Id,
            Phrase = this.Phrase,
            Action = this.Action,
            IsEnabled = this.IsEnabled,
            IsBuiltIn = this.IsBuiltIn
        };
    }

    public enum VoiceCommandAction
    {
        Screenshot,
        OpenSettings,
        Exit,
        VoiceOn,
        VoiceOff
    }
}
