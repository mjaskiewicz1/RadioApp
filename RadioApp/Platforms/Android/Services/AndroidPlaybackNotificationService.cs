using Android.Content;

using RadioApp.Interfaces;


using AndroidApplication = Android.App.Application;

// ReSharper disable once CheckNamespace
namespace RadioApp.Platforms.Android.Services;

public sealed class AndroidPlaybackNotificationService : IPlaybackNotificationService
{
    public void Start()
    {
        var context = AndroidApplication.Context;
        context.StartForegroundService(new Intent(context, typeof(MediaPlaybackService)));
    }

    public void Stop()
    {
        var context = AndroidApplication.Context;
        context.StopService(new Intent(context, typeof(MediaPlaybackService)));
    }
}