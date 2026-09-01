using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Interfaces;
using Core.Models;

using RadioBrowser.Exceptions;

namespace RadioApp.ViewModels;

public partial class MainViewModel(IRadioDirectoryService radioDirectoryService) : ObservableObject
{
    [ObservableProperty] public partial ImmutableList<RadioStation> RecommendedStations { get; private set; } = [];
    [ObservableProperty] public partial bool HasRecommendedError { get; private set; }
    [ObservableProperty] public partial string? RecommendedErrorMessage { get; private set; }

    [RelayCommand]
    public async Task LoadRecommendedAsync()
    {
        try
        {
            HasRecommendedError = false;
            RecommendedErrorMessage = null;
            RecommendedStations = await radioDirectoryService.GetRecommendedAsync();
        }
        catch (RadioBrowserException)
        {
            HasRecommendedError = true;
            RecommendedErrorMessage = "Nie udało się pobrać polecanych stacji.";
        }
        catch (Exception)
        {
            HasRecommendedError = true;
            RecommendedErrorMessage = "Wystąpił nieoczekiwany błąd.";
        }
    }
}