using System.Collections.Immutable;
using Core.Models;

using RadioBrowser.Api.Models.Enums;

namespace Core.Interfaces;

public interface IRadioDirectoryService
{
    Task<IEnumerable<RadioStation>> GetRecommendedAsync();
    Task<IEnumerable<RadioStation>> SearchAsync(string name, CountryCode countryCode);
}