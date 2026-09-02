using Core.Models;

using Microsoft.Extensions.Logging;

using VlcLibVLC = LibVLCSharp.Shared.LibVLC;
using VlcMedia = LibVLCSharp.Shared.Media;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace RadioApp.Views.Components;

public partial class PlayerView
{
    private readonly ILogger<PlayerView> _logger;
    private readonly VlcLibVLC? _libVlc;
    private readonly VlcMediaPlayer? _mediaPlayer;

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

        _logger = IPlatformApplication.Current?.Services.GetRequiredService<ILogger<PlayerView>>()
                  ?? throw new InvalidOperationException("Application services are not available.");

        try
        {
            _libVlc = new VlcLibVLC();
            _mediaPlayer = new VlcMediaPlayer(_libVlc);

            _mediaPlayer.Playing += (_, _) => MainThread.BeginInvokeOnMainThread(() => IsPlaying = true);
            _mediaPlayer.Paused += (_, _) => MainThread.BeginInvokeOnMainThread(() => IsPlaying = false);
            _mediaPlayer.Stopped += (_, _) => MainThread.BeginInvokeOnMainThread(() => IsPlaying = false);
            _mediaPlayer.EndReached += (_, _) => MainThread.BeginInvokeOnMainThread(() => IsPlaying = false);
            _mediaPlayer.EncounteredError += OnPlaybackError;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Failed to initialize LibVLC.");
            ShowError("Błąd odtwarzacza", "Nie udało się uruchomić odtwarzacza.");
        }
    }

    private static void OnCurrentStationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not PlayerView playerView || newValue is not RadioStation station)
            return;

        if (playerView._libVlc is null || playerView._mediaPlayer is null)
            return;

        playerView._playbackErrorShown = false;
        playerView.IsPlaying = false;

        try
        {
            if (playerView._mediaPlayer.IsPlaying)
                playerView._mediaPlayer.Stop();

            var media = new VlcMedia(playerView._libVlc, station.StreamUrl, ":no-video");
            playerView._mediaPlayer.Media = media;

            if (playerView._mediaPlayer.Play())
                return;

            playerView._logger.LogError("LibVLC failed to start station {StationName} from {StreamUrl}.", station.Name,
                station.StreamUrl);

            playerView.ShowPlaybackError();
        }
        catch (Exception exception)
        {
            playerView._logger.LogError(exception, "Failed to play station {StationName} from {StreamUrl}.",
                station.Name, station.StreamUrl);

            playerView.ShowPlaybackError();
        }
    }

    private void OnPlaybackButtonTapped(object? sender, TappedEventArgs e)
    {
        if (_mediaPlayer is null || CurrentStation is null)
            return;

        try
        {
            if (IsPlaying)
            {
                _mediaPlayer.Pause();
                return;
            }

            if (!_mediaPlayer.Play())
                ShowPlaybackError();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to change playback state for station {StationName}.",
                CurrentStation.Name);

            ShowPlaybackError();
        }
    }

    private void OnPlaybackError(object? sender, EventArgs eventArgs)
    {
        _logger.LogError("LibVLC encountered an error while playing station {StationName} from {StreamUrl}.",
            CurrentStation?.Name, CurrentStation?.StreamUrl);

        MainThread.BeginInvokeOnMainThread(ShowPlaybackError);
    }

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