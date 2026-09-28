using Core.Interfaces;
using Core.Models;

namespace RadioApp.Views.Components;

public partial class PlayerView
{
    private readonly IRadioPlaybackService _playbackService;
    private bool _playbackErrorShown;

    public static readonly BindableProperty CurrentStationProperty =
        BindableProperty.Create(propertyName: nameof(CurrentStation), returnType: typeof(RadioStation),
            declaringType: typeof(PlayerView), defaultValue: null, propertyChanged: OnCurrentStationChanged);

    public static readonly BindableProperty IsPlayingProperty =
        BindableProperty.Create(propertyName: nameof(IsPlaying), returnType: typeof(bool),
            declaringType: typeof(PlayerView), defaultValue: false);

    public RadioStation? CurrentStation
    {
        get => (RadioStation?)GetValue(CurrentStationProperty);
        set => SetValue(CurrentStationProperty, value);
    }

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        private set => SetValue(IsPlayingProperty, value);
    }

    public PlayerView()
    {
        InitializeComponent();

        _playbackService = IPlatformApplication.Current?.Services.GetRequiredService<IRadioPlaybackService>()
                           ?? throw new InvalidOperationException("Application services are not available.");

        _playbackService.StateChanged += OnPlaybackStateChanged;
        _playbackService.InitializationFailed += OnInitializationFailed;
        _playbackService.PlaybackFailed += OnPlaybackFailed;
        IsPlaying = _playbackService.IsPlaying;
    }

    private static void OnCurrentStationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not PlayerView playerView || newValue is not RadioStation station)
            return;

        playerView._playbackErrorShown = false;
        playerView.IsPlaying = false;
        playerView._playbackService.Play(station);
    }

    private void OnPlaybackButtonTapped(object? sender, TappedEventArgs e)
    {
        if (CurrentStation is null)
            return;

        if (_playbackService.IsPlaying)
        {
            _playbackService.Pause();
            return;
        }

        _playbackService.Resume();
    }

    private void OnPlaybackStateChanged(object? sender, EventArgs e)
        => MainThread.BeginInvokeOnMainThread(() => IsPlaying = _playbackService.IsPlaying);

    private static void OnInitializationFailed(object? sender, EventArgs e)
        => ShowError("Błąd odtwarzacza", "Nie udało się uruchomić odtwarzacza.");

    private void OnPlaybackFailed(object? sender, EventArgs e)
        => MainThread.BeginInvokeOnMainThread(ShowPlaybackError);

    private void ShowPlaybackError()
    {
        if (_playbackErrorShown)
            return;

        _playbackErrorShown = true;
        IsPlaying = false;

        ShowError("Nie udało się uruchomić stacji", "Wybrana stacja jest obecnie niedostępna. Wybierz inną stację.");
    }

    private static void ShowError(string title, string message)
    {
        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current is not null)
                await Shell.Current.DisplayAlertAsync(title, message, "OK");
        });
    }
}
