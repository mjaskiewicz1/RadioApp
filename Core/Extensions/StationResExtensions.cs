using Core.Models;

using RadioBrowser.Api.Models.Response;

namespace Core.Extensions;

public static class StationResExtensions
{
    extension(IEnumerable<StationRes> stations)
    {
        public IEnumerable<RadioStation> ToRadioStations()
        {
            return stations
                .DistinctBy(static x => x.UrlResolved)
                .Select(ToRadioStation);
        }
    }

    private static RadioStation ToRadioStation(StationRes station) => new(station.StationUuid, station.Name, station.UrlResolved, station.Favicon, station.Bitrate);
}