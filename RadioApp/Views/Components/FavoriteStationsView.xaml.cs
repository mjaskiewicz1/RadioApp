using Core.Data.Entities;

using RadioApp.Events;

namespace RadioApp.Views.Components;

public partial class FavoriteStationsView
{
    public event EventHandler<StationSelectedEventArgs>? StationSelected;
    public event EventHandler<StationSelectedEventArgs>? FavoriteAddRequested;
    public event EventHandler<FavoriteStationEventArgs>? StationRemoveRequested;

    public FavoriteStationsView()
    {
        InitializeComponent();
    }

    private void OnStationSelected(object? sender, StationSelectedEventArgs e)
        => StationSelected?.Invoke(this, e);

    private void OnFavoriteAddRequested(object? sender, StationSelectedEventArgs e)
        => FavoriteAddRequested?.Invoke(this, e);

    private void OnRemoveTapped(object? sender, EventArgs e)
    {
        if (sender is not ImageButton { BindingContext: FavoriteStation station })
            return;

        StationRemoveRequested?.Invoke(this, new FavoriteStationEventArgs(station));
    }
}