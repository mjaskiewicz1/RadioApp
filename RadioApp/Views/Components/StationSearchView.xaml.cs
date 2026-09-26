using System.Collections.ObjectModel;

using Core.Data.Entities;
using Core.Models;

using RadioApp.Events;

namespace RadioApp.Views.Components;

public partial class StationSearchView
{
    public event EventHandler<StationSelectedEventArgs>? StationSelected;
    public event EventHandler<StationSelectedEventArgs>? FavoriteAddRequested;
    public event EventHandler<StationSelectedEventArgs>? FavoriteToggleRequested;

    public static readonly BindableProperty FavoriteStationsProperty =
        BindableProperty.Create(nameof(FavoriteStations), typeof(ObservableCollection<FavoriteStation>),
            typeof(StationSearchView));

    public ObservableCollection<FavoriteStation>? FavoriteStations
    {
        get => (ObservableCollection<FavoriteStation>?)GetValue(FavoriteStationsProperty);
        set => SetValue(FavoriteStationsProperty, value);
    }

    public StationSearchView()
    {
        InitializeComponent();
    }

    private void OnStationSelected(object? sender, StationSelectedEventArgs e)
        => StationSelected?.Invoke(this, e);

    private void OnFavoriteAddRequested(object? sender, StationSelectedEventArgs e)
        => FavoriteAddRequested?.Invoke(this, e);

    private void OnFavoriteTapped(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: RadioStation station })
            return;

        FavoriteToggleRequested?.Invoke(this, new StationSelectedEventArgs(station));
    }

    private  void OnSearchEntryHandlerChanged(object? sender, EventArgs e)
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