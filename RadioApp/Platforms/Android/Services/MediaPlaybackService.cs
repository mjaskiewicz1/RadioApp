using _Microsoft.Android.Resource.Designer;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Media.Session;
using Android.OS;

using RadioApp.Interfaces;

// ReSharper disable once CheckNamespace
namespace RadioApp.Platforms.Android;

[Service(Exported = false, ForegroundServiceType = ForegroundService.TypeMediaPlayback)]
public class MediaPlaybackService : Service
{
    private const int NotificationId = 1001;
    private const string ChannelId = "radio_playback";

    private IRadioPlaybackService? _playbackService;
    private MediaSession? _mediaSession;
    private PlaybackCallback? _callback;
    private NotificationManager? _notificationManager;

    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnCreate()
    {
        base.OnCreate();

        _playbackService = IPlatformApplication.Current?.Services.GetRequiredService<IRadioPlaybackService>()
                           ?? throw new InvalidOperationException("Application services are not available.");

        _playbackService.StateChanged += OnPlaybackStateChanged;

        _notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
        _notificationManager.CreateNotificationChannel(
            new NotificationChannel(ChannelId, "Odtwarzanie radia", NotificationImportance.Low));

        _mediaSession = new MediaSession(this, "RadioApp");
        _callback = new PlaybackCallback(_playbackService);
        _mediaSession.SetCallback(_callback);
        _mediaSession.SetSessionActivity(CreateAppIntent());
        _mediaSession.Active = true;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        if (_playbackService?.CurrentStation is null)
        {
            StopSelf(startId);
            return StartCommandResult.NotSticky;
        }

        UpdateMediaSession();
        StartForeground(NotificationId, BuildNotification());

        if (intent?.Action == nameof(TogglePlayback))
            TogglePlayback();

        return StartCommandResult.NotSticky;
    }

    public override void OnDestroy()
    {
        _playbackService?.StateChanged -= OnPlaybackStateChanged;

        StopForeground(StopForegroundFlags.Remove);
        _mediaSession?.Release();
        _callback?.Dispose();

        base.OnDestroy();
    }

    private void OnPlaybackStateChanged(object? sender, EventArgs e)
    {
        if (_playbackService?.CurrentStation is null)
            return;

        UpdateMediaSession();
        _notificationManager?.Notify(NotificationId, BuildNotification());
    }

    private void UpdateMediaSession()
    {
        if (_playbackService?.CurrentStation is not { } station || _mediaSession is null)
            return;

        var metadata = new MediaMetadata.Builder()
            .PutString(MediaMetadata.MetadataKeyTitle, station.Name)!
            .PutString(MediaMetadata.MetadataKeyArtist, "RadioApp")!
            .Build();

        var playbackState = new PlaybackState.Builder()
            .SetActions(PlaybackState.ActionPlay | PlaybackState.ActionPause)!
            .SetState(
                _playbackService.IsPlaying ? PlaybackStateCode.Playing : PlaybackStateCode.Paused,
                PlaybackState.PlaybackPositionUnknown,
                _playbackService.IsPlaying ? 1f : 0f)!
            .Build();

        _mediaSession.SetMetadata(metadata);
        _mediaSession.SetPlaybackState(playbackState);
    }

    private Notification BuildNotification()
    {
        var isPlaying = _playbackService!.IsPlaying;

        var actionIntent = new Intent(this, typeof(MediaPlaybackService))
            .SetAction(nameof(TogglePlayback));

        var actionPendingIntent = PendingIntent.GetService(this, 1, actionIntent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        using var icon = Icon.CreateWithResource(this,
            isPlaying ? ResourceConstant.Drawable.media_pause : ResourceConstant.Drawable.media_play);

        var control =
            new Notification.Action.Builder(icon, isPlaying ? "Pauza" : "Odtwórz", actionPendingIntent).Build();

        return new Notification.Builder(this, ChannelId)
            .SetSmallIcon(ResourceConstant.Drawable.radio_notification)
            .SetContentTitle(_playbackService.CurrentStation!.Name)
            .SetContentText(isPlaying ? "Odtwarzanie" : "Wstrzymano")
            .SetContentIntent(CreateAppIntent())
            .SetVisibility(NotificationVisibility.Public)
            .SetOnlyAlertOnce(true)
            .SetOngoing(true)
            .AddAction(control)
            .SetStyle(new Notification.MediaStyle()
                .SetMediaSession(_mediaSession!.SessionToken)!
                .SetShowActionsInCompactView(0))
            .Build();
    }

    private void TogglePlayback()
    {
        if (_playbackService?.IsPlaying == true)
            _playbackService.Pause();
        else
            _playbackService?.Resume();
    }

    private PendingIntent? CreateAppIntent()
    {
        var intent = new Intent(this, typeof(MainActivity))
            .SetAction(Intent.ActionMain)
            .AddCategory(Intent.CategoryLauncher)
            .SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        return PendingIntent.GetActivity(this, 0, intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
    }

    private sealed class PlaybackCallback(IRadioPlaybackService playbackService) : MediaSession.Callback
    {
        public override void OnPlay()
            => MainThread.BeginInvokeOnMainThread(playbackService.Resume);

        public override void OnPause()
            => MainThread.BeginInvokeOnMainThread(playbackService.Pause);
    }
}