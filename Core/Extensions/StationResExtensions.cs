using System.Collections.Immutable;

using Core.Models;

using RadioBrowser.Api.Models.Response;

namespace Core.Extensions;

public static class StationResExtensions
{
    extension(ImmutableList<StationRes> stations)
    {
        public ImmutableList<RadioStation> ToRadioStations()
            => [.. stations.DistinctBy(static x => x.UrlResolved).Select(ToRadioStation)];
    }

    private static RadioStation ToRadioStation(StationRes station)
        => new(station.StationUuid, station.Name, station.UrlResolved, station.Favicon, station.Bitrate);
}