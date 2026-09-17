using Core.Models;

namespace RadioApp.Events;

public sealed class StationSelectedEventArgs(RadioStation station) : EventArgs
{
    public RadioStation Station { get; } = station;
}