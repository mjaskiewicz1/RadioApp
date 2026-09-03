using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Interfaces;
using Core.Models;

using Microsoft.Extensions.Logging;

using RadioBrowser.Api.Models.Enums;
using RadioBrowser.Exceptions;

namespace RadioApp.ViewModels;

public partial class SearchViewModel(IRadioDirectoryService radioDirectoryService, ILogger<SearchViewModel> logger)
    : ObservableObject
{
    [ObservableProperty] public partial string? SearchQuery { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResults))]
    public partial ImmutableList<RadioStation> SearchResults { get; private set; } = [];

    [ObservableProperty] public partial bool HasError { get; private set; }
    [ObservableProperty] public partial string? ErrorMessage { get; private set; }

    public bool HasResults => SearchResults.Count > 0;

    [RelayCommand]
    public async Task SearchAsync(CountryCode countryCode)
    {
        var query = SearchQuery?.Trim();

        if (string.IsNullOrEmpty(query) || query.Length < 3)
        {
            SearchResults = [];
            HasError = false;
            ErrorMessage = null;
            return;
        }

        try
        {
            HasError = false;
            ErrorMessage = null;
            SearchResults = [];

            SearchResults = await radioDirectoryService.SearchAsync(query, countryCode);
        }
        catch (RadioBrowserException exception)
        {
            logger.LogError(exception, "Failed to search radio stations for {SearchQuery}.", query);

            SearchResults = [];
            HasError = true;
            ErrorMessage = "Nie udało się wyszukać stacji.";
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Unexpected error while searching radio stations for {SearchQuery}.", query);

            SearchResults = [];
            HasError = true;
            ErrorMessage = "Wystąpił nieoczekiwany błąd.";
        }
    }
}