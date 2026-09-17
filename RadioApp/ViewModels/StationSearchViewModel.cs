using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Interfaces;
using Core.Models;

using Microsoft.Extensions.Logging;

using RadioBrowser.Api.Models.Enums;

namespace RadioApp.ViewModels;

public partial class StationSearchViewModel(
    IRadioDirectoryService radioDirectoryService,
    ILogger<StationSearchViewModel> logger) : ObservableObject
{
    private const int MinimumSearchLength = 3;
    private const int SearchDelayMilliseconds = 350;

    private int _searchVersion;

    [ObservableProperty] public partial bool IsSearchVisible { get; private set; }
    [ObservableProperty] public partial bool IsSearching { get; private set; }
    [ObservableProperty] public partial string SearchText { get; set; } = string.Empty;
    [ObservableProperty] public partial CountryCode SelectedCountry { get; private set; } = CountryCode.Pl;
    [ObservableProperty] public partial ImmutableList<RadioStation> Results { get; private set; } = [];

    [RelayCommand]
    private void OpenSearch()
        => IsSearchVisible = true;

    [RelayCommand]
    private void CloseSearch()
    {
        IsSearchVisible = false;
        SearchText = string.Empty;
    }

    [RelayCommand]
    private void SelectCountry(CountryCode countryCode)
    {
        if (countryCode is not CountryCode.Pl and not CountryCode.Gb)
            return;

        SelectedCountry = countryCode;
        StartSearch(SearchText);
    }

    partial void OnSearchTextChanged(string value)
        => StartSearch(value);

    private void StartSearch(string searchText)
    {
        var searchVersion = ++_searchVersion;
        var query = searchText.Trim();

        Results = [];
        IsSearching = false;

        if (query.Length < MinimumSearchLength)
            return;

        _ = SearchAsync(query, SelectedCountry, searchVersion);
    }

    private async Task SearchAsync(string query, CountryCode countryCode, int searchVersion)
    {
        try
        {
            await Task.Delay(SearchDelayMilliseconds);

            if (searchVersion != _searchVersion)
                return;

            IsSearching = true;

            var stations = await radioDirectoryService.SearchAsync(query, countryCode);

            if (searchVersion != _searchVersion)
                return;

            Results = [.. stations];
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to search radio stations.");
        }
        finally
        {
            if (searchVersion == _searchVersion)
                IsSearching = false;
        }
    }
}