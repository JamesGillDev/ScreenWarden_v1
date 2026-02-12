using System;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using ScreenWarden.Models;
using ScreenWarden.Services;

namespace ScreenWarden
{
    public sealed class VoiceCommandService : IDisposable
    {
        private SpeechRecognitionEngine? _engine;
        private VoiceCommandsSettings _settings;
        private CultureInfo? _culture;

        public bool IsEnabled { get; private set; } = true;

        public event Action? ScreenshotRequested;
        public event Action? OpenSettingsRequested;
        public event Action? ExitRequested;
        public event Action<bool>? VoiceStateChanged;

        public VoiceCommandService()
        {
            _settings = VoiceCommandsSettings.Load();
        }

        public VoiceCommandsSettings GetSettings() => _settings;

        public void UpdateSettings(VoiceCommandsSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settings = settings;
            _settings.Save();
            ReloadGrammar();
        }

        public void Start()
        {
            if (_engine != null) return;

            // Force culture to en-US for testing
            _culture = new CultureInfo("en-US");
            _engine = new SpeechRecognitionEngine(_culture);

            LoadGrammarFromSettings();

            _engine.SetInputToDefaultAudioDevice();
            _engine.SpeechRecognized += Engine_SpeechRecognized;
            _engine.RecognizeAsync(RecognizeMode.Multiple);

            VoiceStateChanged?.Invoke(IsEnabled);
        }

        private void LoadGrammarFromSettings()
        {
            if (_engine == null || _culture == null) return;

            var enabledPhrases = _settings.Commands
                .Where(c => c.IsEnabled && !string.IsNullOrWhiteSpace(c.Phrase))
                .Select(c => c.Phrase.ToLowerInvariant())
                .Distinct()
                .ToArray();

            if (enabledPhrases.Length == 0)
            {
                enabledPhrases = new[] { "screen warden placeholder" };
            }

            var commands = new Choices(enabledPhrases);
            var grammarBuilder = new GrammarBuilder(commands) { Culture = _culture };
            var grammar = new Grammar(grammarBuilder);

            _engine.UnloadAllGrammars();
            _engine.LoadGrammar(grammar);
        }

        public void ReloadGrammar()
        {
            if (_engine == null) return;

            try
            {
                _engine.RecognizeAsyncCancel();
                LoadGrammarFromSettings();
                _engine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                // Ignore reload errors
            }
        }

        public void Enable()
        {
            if (IsEnabled) return;
            IsEnabled = true;
            VoiceStateChanged?.Invoke(true);
        }

        public void Disable()
        {
            if (!IsEnabled) return;
            IsEnabled = false;
            VoiceStateChanged?.Invoke(false);
        }

        public void Toggle()
        {
            if (IsEnabled) Disable();
            else Enable();
        }

        private void Engine_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            // Log recognized text and confidence
            Console.WriteLine($"Recognized: '{e.Result.Text}' Confidence: {e.Result.Confidence}");

            // Lower threshold for testing
            if (e.Result.Confidence < 0.50) return;

            var text = e.Result.Text.ToLowerInvariant();

            var command = _settings.Commands
                .FirstOrDefault(c => c.IsEnabled && c.Phrase.Equals(text, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                Console.WriteLine("No matching command found.");
                return;
            }

            if (command.Action == VoiceCommandAction.VoiceOn)
            {
                Enable();
                return;
            }

            if (command.Action == VoiceCommandAction.VoiceOff)
            {
                Disable();
                return;
            }

            if (!IsEnabled) return;

            switch (command.Action)
            {
                case VoiceCommandAction.Screenshot:
                    ScreenshotRequested?.Invoke();
                    break;
                case VoiceCommandAction.OpenSettings:
                    OpenSettingsRequested?.Invoke();
                    break;
                case VoiceCommandAction.Exit:
                    ExitRequested?.Invoke();
                    break;
            }
        }

        public void Stop()
        {
            if (_engine == null) return;

            try
            {
                _engine.SpeechRecognized -= Engine_SpeechRecognized;
                _engine.RecognizeAsyncCancel();
                _engine.RecognizeAsyncStop();
            }
            catch { }

            _engine.Dispose();
            _engine = null;
        }

        public void Dispose() => Stop();
    }
}
