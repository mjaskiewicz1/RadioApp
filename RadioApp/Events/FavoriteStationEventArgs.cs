using Core.Data.Entities;

namespace RadioApp.Events;

public sealed class FavoriteStationEventArgs(FavoriteStation station) : EventArgs
{
    public FavoriteStation Station { get; } = station;
}