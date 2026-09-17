using Core.Models;

using RadioApp.Events;

namespace RadioApp.Views.Components;

public partial class StationSearchView
{
    public event EventHandler<StationSelectedEventArgs>? StationSelected;

    public StationSearchView()
    {
        InitializeComponent();
    }

    private void OnStationTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid { BindingContext: RadioStation station })
            return;

        StationSelected?.Invoke(this, new StationSelectedEventArgs(station));
    }


    private void OnSearchEntryHandlerChanged(object? sender, EventArgs e)
    {
#if ANDROID
        if (sender is not Entry entry ||
            entry.Handler?.PlatformView is not Android.Widget.EditText platformView)
            return;

        platformView.BackgroundTintList =
            Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
    }
}