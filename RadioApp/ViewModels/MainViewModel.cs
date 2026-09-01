using System.Collections.Immutable;

using CommunityToolkit.Mvvm.ComponentModel;

using Core.Interfaces;
using Core.Models;

namespace RadioApp.ViewModels;

public partial class MainViewModel(IRadioDirectoryService radioDirectoryService) : ObservableObject
{
    [ObservableProperty]
    public partial ImmutableList<RadioStation> RecommendedStations { get; private set; } = [];

    public async Task LoadRecommendedAsync()
        => RecommendedStations = await radioDirectoryService.GetRecommendedAsync();
}