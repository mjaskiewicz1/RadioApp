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
            => [.. stations
                .GroupBy(static station => station.UrlResolved)
                .Select(static group => SelectBestStation(group))
                .GroupBy(static station => station.Name, StringComparer.OrdinalIgnoreCase)
                .Select(static group => SelectBestStation(group))
                .Select(ToRadioStation)];
    }

    private static StationRes SelectBestStation(IEnumerable<StationRes> stations)
        => stations
            .OrderByDescending(static station => GetCodecPriority(station.Codec))
            .First();

    private static RadioStation ToRadioStation(StationRes station)
        => new(station.StationUuid, station.Name, station.UrlResolved, GetFavicon(station.Favicon), station.Bitrate);

    private static int GetCodecPriority(string codec)
        => codec.ToUpperInvariant() switch
        {
            "AAC+" => 2,
            "AAC" => 1,
            _ => 0
        };

    private static Uri? GetFavicon(Uri? favicon)
    {
        return favicon is null ? null : !SupportedFaviconExtensions.Contains(Path.GetExtension(favicon.AbsolutePath)) ? null : favicon;
    }
}