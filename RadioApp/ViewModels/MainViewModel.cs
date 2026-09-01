using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;

using Core.Interfaces;
using Core.Models;

namespace RadioApp.ViewModels;

public class MainViewModel(IRadioDirectoryService radioDirectoryService) : ObservableObject
{
    public ImmutableList<RadioStation> RecommendedStations
    {
        get;
        private set => SetProperty(ref field, value);
    } = [];

    public async Task LoadRecommendedAsync()
        => RecommendedStations = await radioDirectoryService.GetRecommendedAsync();
}