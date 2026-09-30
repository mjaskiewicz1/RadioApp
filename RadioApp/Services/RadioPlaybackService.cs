using Core.Models;

using Microsoft.Extensions.Logging;

using RadioApp.Interfaces;

using VlcLibVLC = LibVLCSharp.Shared.LibVLC;
using VlcMedia = LibVLCSharp.Shared.Media;
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace RadioApp.Services;

public sealed class RadioPlaybackService(
    ILogger<RadioPlaybackService> logger,
    IPlaybackNotificationService playbackNotificationService) : IRadioPlaybackService, IDisposable
{
    private VlcLibVLC? _libVlc;
    private VlcMediaPlayer? _mediaPlayer;
    private VlcMedia? _media;
    private bool _disposed;
    private bool _notificationStarted;

    public RadioStation? CurrentStation { get; private set; }
    public bool IsPlaying { get; private set; }

    public event EventHandler? StateChanged;
    public event EventHandler? InitializationFailed;
    public event EventHandler? PlaybackFailed;

    public void Play(RadioStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        UpdateState(station, false);

        if (!TryInitialize())
            return;

        try
        {
            StartNotification();

            if (_mediaPlayer!.IsPlaying)
                _mediaPlayer.Stop();

            var media = new VlcMedia(_libVlc!, station.StreamUrl, ":no-video");

            try
            {
                _mediaPlayer.Media = media;
            }
            catch
            {
                media.Dispose();
                throw;
            }

            _media?.Dispose();
            _media = media;

            if (!_mediaPlayer.Play())
                ReportPlaybackFailure();
        }
        catch (Exception exception)
        {
            ReportPlaybackFailure(exception);
        }
    }

    public void Resume()
    {
        if (_mediaPlayer is null || CurrentStation is null)
            return;

        try
        {
            StartNotification();

            if (!_mediaPlayer.Play())
                ReportPlaybackFailure();
        }
        catch (Exception exception)
        {
            ReportPlaybackFailure(exception);
        }
    }

    public void Pause()
    {
        if (_mediaPlayer is null || !IsPlaying)
            return;

        try
        {
            _mediaPlayer.Pause();
        }
        catch (Exception exception)
        {
            ReportPlaybackFailure(exception);
        }
    }

    public void Stop()
    {
        try
        {
            _mediaPlayer?.Stop();
            UpdateState(CurrentStation, false);
        }
        catch (Exception exception)
        {
            ReportPlaybackFailure(exception);
        }
        finally
        {
            StopNotification();
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        StopNotification();
        _mediaPlayer?.Dispose();
        _media?.Dispose();
        _libVlc?.Dispose();
    }

    private bool TryInitialize()
    {
        if (_mediaPlayer is not null)
            return true;

        try
        {
            _libVlc = new VlcLibVLC();
            _mediaPlayer = new VlcMediaPlayer(_libVlc);

            _mediaPlayer.Playing += (_, _) => DispatchState(true);
            _mediaPlayer.Paused += (_, _) => DispatchState(false);
            _mediaPlayer.Stopped += (_, _) => DispatchState(false);
            _mediaPlayer.EndReached += (_, _) => DispatchState(false);
            _mediaPlayer.EncounteredError += (_, _) =>
                MainThread.BeginInvokeOnMainThread(() => ReportPlaybackFailure());

            return true;
        }
        catch (Exception exception)
        {
            _mediaPlayer?.Dispose();
            _libVlc?.Dispose();
            _mediaPlayer = null;
            _libVlc = null;

            logger.LogCritical(exception, "Failed to initialize LibVLC.");
            InitializationFailed?.Invoke(this, EventArgs.Empty);
            return false;
        }
    }

    private void DispatchState(bool isPlaying)
        => MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!_disposed)
                UpdateState(CurrentStation, isPlaying);
        });

    private void UpdateState(RadioStation? station, bool isPlaying)
    {
        if (CurrentStation == station && IsPlaying == isPlaying)
            return;

        CurrentStation = station;
        IsPlaying = isPlaying;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void StartNotification()
    {
        if (_notificationStarted)
            return;

        playbackNotificationService.Start();
        _notificationStarted = true;
    }

    private void StopNotification()
    {
        if (!_notificationStarted)
            return;

        _notificationStarted = false;
        playbackNotificationService.Stop();
    }

    private void ReportPlaybackFailure(Exception? exception = null)
    {
        if (_disposed)
            return;

        logger.LogError(exception, "Failed to play station {StationName} from {StreamUrl}.",
            CurrentStation?.Name, CurrentStation?.StreamUrl);

        UpdateState(CurrentStation, false);
        StopNotification();
        PlaybackFailed?.Invoke(this, EventArgs.Empty);
    }
}