using System.Collections.Immutable;

using Core.Models;

using RadioBrowser.Api.Models.Response;

namespace Core.Extensions;

public static class StationResExtensions
{
    private static readonly HashSet<string> SupportedFaviconExtensions = [".png", ".jpg", ".jpeg", ".webp"];

    extension(ImmutableList<StationRes> stations)
    {
        public ImmutableList<RadioStation> ToRadioStations()
            => [.. stations.DistinctBy(static x => x.UrlResolved).Select(ToRadioStation)];
    }


    private static RadioStation ToRadioStation(StationRes station)
        => new(station.StationUuid, station.Name, station.UrlResolved, GetFavicon(station.Favicon), station.Bitrate);

    private static Uri? GetFavicon(Uri? favicon)
    {
        return favicon is null ? null : !SupportedFaviconExtensions.Contains(Path.GetExtension(favicon.AbsolutePath)) ? null : favicon;
    }
}