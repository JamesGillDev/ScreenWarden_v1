using System;
using System.Globalization;
using System.Speech.Recognition;

namespace ScreenWarden;

public sealed class VoiceCommandService : IDisposable
{
    private SpeechRecognitionEngine? _engine;

    // If false, we keep the engine alive but ignore commands (fast toggle, no re-init delay).
    public bool IsEnabled { get; private set; } = true;

    // Events the app subscribes to
    public event Action? ScreenshotRequested;
    public event Action? OpenSettingsRequested;
    public event Action? ExitRequested;

    // Fires when Voice On/Off changes (so App.xaml.cs can show tray balloons)
    public event Action<bool>? VoiceStateChanged;

    public void Start()
    {
        if (_engine != null) return;

        // Use Windows UI language
        var culture = CultureInfo.InstalledUICulture;

        _engine = new SpeechRecognitionEngine(culture);

        // Keep commands tight to reduce false triggers
        // (Removed "quit" and "clip" per your request)
        var commands = new Choices(
            "voice on screen warden",
            "voice off screen warden",
            "screenshot",
            "capture",
            "open settings",
            "settings",
            // Make exit require the app name to reduce accidental shutdown
            "exit screen warden"
        );

        var grammarBuilder = new GrammarBuilder(commands) { Culture = culture };
        var grammar = new Grammar(grammarBuilder);

        _engine.LoadGrammar(grammar);

        // Default mic
        _engine.SetInputToDefaultAudioDevice();

        _engine.SpeechRecognized += Engine_SpeechRecognized;
        _engine.RecognizeAsync(RecognizeMode.Multiple);

        // Initial state notification
        VoiceStateChanged?.Invoke(IsEnabled);
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
        // Confidence threshold: raise it if you’re getting accidental triggers
        if (e.Result.Confidence < 0.70) return;

        var text = e.Result.Text.ToLowerInvariant();

        // Voice ON/OFF always allowed (even when disabled)
        if (text == "voice on screen warden")
        {
            Enable();
            return;
        }

        if (text == "voice off screen warden")
        {
            Disable();
            return;
        }

        // If voice is off, ignore everything else
        if (!IsEnabled) return;

        switch (text)
        {
            case "screenshot":
            case "capture":
                ScreenshotRequested?.Invoke();
                break;

            case "open settings":
            case "settings":
                OpenSettingsRequested?.Invoke();
                break;

            case "exit screen warden":
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
        catch
        {
            // ignore shutdown noise
        }

        _engine.Dispose();
        _engine = null;
    }

    public void Dispose() => Stop();
}
