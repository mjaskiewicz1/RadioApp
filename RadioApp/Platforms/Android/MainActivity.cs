using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

using RadioApp.Startup;

using View = Android.Views.View;

namespace RadioApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var decorView = Window?.DecorView;
        if (decorView is null)
            return;

        decorView.ViewTreeObserver?.AddOnPreDrawListener(new StartupPreDrawListener(decorView));
    }

    private sealed class StartupPreDrawListener(View decorView) : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
    {
        public bool OnPreDraw()
        {
            if (StartupState.IsLoading)
                return false;

            decorView.ViewTreeObserver?.RemoveOnPreDrawListener(this);
            return true;
        }
    }
}