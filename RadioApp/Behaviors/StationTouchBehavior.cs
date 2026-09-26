using CommunityToolkit.Maui.Behaviors;

using Core.Data.Entities;
using Core.Extensions;
using Core.Models;

using RadioApp.Events;

namespace RadioApp.Behaviors;

public sealed class StationTouchBehavior : TouchBehavior
{
    public event EventHandler<StationSelectedEventArgs>? StationSelected;
    public event EventHandler<StationSelectedEventArgs>? FavoriteAddRequested;

    public StationTouchBehavior()
    {
        LongPressDuration = 600;
        DisallowTouchThreshold = 12;
        PressedOpacity = 0.65;
        ShouldMakeChildrenInputTransparent = true;

        // One recognizer handles both gestures; do not add a TapGestureRecognizer to the same view.
        Command = new Command(SelectStation);
        LongPressCommand = new Command(AddFavorite);
    }

    private void SelectStation()
    {
        if (GetStation() is not { } station)
            return;

        StationSelected?.Invoke(this, new StationSelectedEventArgs(station));
    }

    private void AddFavorite()
    {
        if (GetStation() is not { } station)
            return;

        FavoriteAddRequested?.Invoke(this, new StationSelectedEventArgs(station));
    }

    private RadioStation? GetStation()
        => BindingContext switch
        {
            RadioStation station => station,
            FavoriteStation station => station.ToRadioStation(),
            _ => null
        };
}