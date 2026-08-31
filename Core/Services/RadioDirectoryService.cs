using System.Collections.Immutable;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using RadioBrowser;
using RadioBrowser.Api.Models.Enums;
using RadioBrowser.Api.Models.Request;

namespace Core.Services;

public sealed class RadioDirectoryService : IRadioDirectoryService
{
    private readonly RadioBrowserClient _radioBrowserClient = RadioBrowserClient.Factory();
    private const uint MaxResults = 10;

    public async Task<IEnumerable<RadioStation>> GetRecommendedAsync()
    {
        var req = new SearchReq
        {
            CountryCode = CountryCode.Pl,
            HideBroken = true,
            Order = Order.Votes,
            Reverse = true,
            Codec = Codec.Mp3
        };
        return (await _radioBrowserClient.GetStationsAsync(searchReq: req, limit: MaxResults)).ToRadioStations();
    }

    public async Task<IEnumerable<RadioStation>> SearchAsync(string name, CountryCode countryCode)
    {
        var req = new SearchReq
        {
            Name = name,
            CountryCode = countryCode,
            HideBroken = true,
            Order = Order.Votes,
            Reverse = true,
            Codec = Codec.Mp3
        };
        return (await _radioBrowserClient.GetStationsAsync(searchReq: req, limit: MaxResults)).ToRadioStations();
    }
}