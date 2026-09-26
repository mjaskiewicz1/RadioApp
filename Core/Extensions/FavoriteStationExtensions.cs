using Core.Data.Entities;
using Core.Models;

namespace Core.Extensions;

public static class FavoriteStationExtensions
{
    extension(FavoriteStation station)
    {
        public RadioStation ToRadioStation()
            => new(station.ExternalStationId, station.Name, station.StreamUrl, station.Favicon, station.Bitrate);
    }

    extension(RadioStation station)
    {
        public FavoriteStation ToFavoriteStation()
            => new()
            {
                ExternalStationId = station.Id,
                Name = station.Name,
                StreamUrl = station.StreamUrl,
                Favicon = station.Favicon,
                Bitrate = station.Bitrate
            };
    }
}
