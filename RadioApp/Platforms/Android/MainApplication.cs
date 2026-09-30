using Android.App;
using Android.Runtime;

using Microsoft.Maui.Handlers;

using RadioApp.Interfaces;
using RadioApp.Platforms.Android.Services;

namespace RadioApp;

[Application]
public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    protected override MauiApp CreateMauiApp()
    {
        EntryHandler.Mapper.AppendToMapping("RemoveUnderline", (handler, _) =>
            handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent));

        return MauiProgram.CreateMauiApp(services =>
            services.AddSingleton<IPlaybackNotificationService, AndroidPlaybackNotificationService>());
    }
}