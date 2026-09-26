using System.Globalization;

using Core.Data.Entities;

namespace RadioApp.Converters;

public sealed class FavoriteHeartConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // The Count binding refreshes the icon when the shared collection changes.
        if (values is not [Guid stationId, IEnumerable<FavoriteStation> stations, ..])
            return "♡";

        return stations.Any(station => station.ExternalStationId == stationId) ? "♥" : "♡";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException("Favorite icons use one-way bindings.");
}