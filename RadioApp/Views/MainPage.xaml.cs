using System.Diagnostics.CodeAnalysis;

using RadioApp.Events;
using RadioApp.ViewModels;
using RadioApp.Views.Components;

namespace RadioApp.Views;

public partial class MainPage
{
    private readonly MainViewModel _viewModel;
    public StationSearchViewModel SearchViewModel { get; }

    public MainPage(MainViewModel viewModel, StationSearchViewModel searchViewModel)
    {
        _viewModel = viewModel;
        SearchViewModel = searchViewModel;

        InitializeComponent();

        BindingContext = viewModel;
    }

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecommendedAsync();
    }

    private void OnSearchStationSelected(object? sender, StationSelectedEventArgs e)
    {
        _viewModel.SelectedStation = e.Station;
    }
}