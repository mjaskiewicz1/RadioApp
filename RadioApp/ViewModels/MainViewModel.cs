using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;

using Core.Interfaces;
using Core.Models;

namespace RadioApp.ViewModels;

public class MainViewModel(IRadioDirectoryService radioDirectoryService) : ObservableObject
{
    private ImmutableList<RadioStation> _recommendedStations = [];

    public ImmutableList<RadioStation> RecommendedStations
    {
        get => _recommendedStations;
        private set => SetProperty(ref _recommendedStations, value);
    }
    
    public async Task LoadRecommendedAsync()
        => _recommendedStations = await radioDirectoryService.GetRecommendedAsync();
}