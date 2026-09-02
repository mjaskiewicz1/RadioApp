using Core.Models;

using RadioApp.ViewModels;

namespace RadioApp.Views.Components;

public partial class RecommendedStationsView
{
    public RecommendedStationsView()
    {
        InitializeComponent();
    }

    private void OnStationTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Grid { BindingContext: RadioStation station })
            return;

        if (BindingContext is MainViewModel viewModel)
            viewModel.SelectedStation = station;
    }
}