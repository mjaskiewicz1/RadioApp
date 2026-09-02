using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Interfaces;
using Core.Models;

using Microsoft.Extensions.Logging;

using RadioApp.Startup;

using RadioBrowser.Exceptions;

namespace RadioApp.ViewModels;

public partial class MainViewModel(IRadioDirectoryService radioDirectoryService, ILogger<MainViewModel> logger) : ObservableObject
{
    [ObservableProperty] public partial ImmutableList<RadioStation> RecommendedStations { get; private set; } = [];
    [ObservableProperty] public partial bool HasRecommendedError { get; private set; }
    [ObservableProperty] public partial string? RecommendedErrorMessage { get; private set; }
    [ObservableProperty] public partial RadioStation? SelectedStation { get; set; }

    [RelayCommand]
    public async Task LoadRecommendedAsync()
    {
        try
        {
            HasRecommendedError = false;
            RecommendedErrorMessage = null;
            RecommendedStations = await radioDirectoryService.GetRecommendedAsync();
        }
        catch (RadioBrowserException exception)
        {
            logger.LogError(exception, "Failed to load recommended radio stations from Radio Browser.");

            HasRecommendedError = true;
            RecommendedErrorMessage = "Nie udało się pobrać polecanych stacji.";
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Unexpected error while loading recommended radio stations.");

            HasRecommendedError = true;
            RecommendedErrorMessage = "Wystąpił nieoczekiwany błąd.";
        }
        finally
        {
            StartupState.IsLoading = false;
        }
    }
}