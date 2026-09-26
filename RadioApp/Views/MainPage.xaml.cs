using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using RadioApp.Events;
using RadioApp.ViewModels;

namespace RadioApp.Views;

public partial class MainPage
{
    private readonly MainViewModel _viewModel;
    private readonly ILogger<MainPage> _logger;
    private int _feedbackVersion;

    public StationSearchViewModel SearchViewModel { get; }
    public FavoriteStationsViewModel FavoriteStationsViewModel { get; }

    public MainPage(MainViewModel viewModel, StationSearchViewModel searchViewModel,
        FavoriteStationsViewModel favoriteStationsViewModel, ILogger<MainPage> logger)
    {
        _viewModel = viewModel;
        _logger = logger;
        SearchViewModel = searchViewModel;
        FavoriteStationsViewModel = favoriteStationsViewModel;

        InitializeComponent();
        BindingContext = viewModel;
    }

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.WhenAll(_viewModel.LoadRecommendedAsync(), LoadFavoritesAsync());
    }

    private void OnStationSelected(object? sender, StationSelectedEventArgs e)
        => _viewModel.SelectedStation = e.Station;

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    private async void OnFavoriteAddRequested(object? sender, StationSelectedEventArgs e)
        => await UpdateFavoritesAsync(async () => await FavoriteStationsViewModel.AddAsync(e.Station)
            ? "Dodano do ulubionych"
            : "Stacja jest już w ulubionych");

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    private async void OnFavoriteToggleRequested(object? sender, StationSelectedEventArgs e)
        => await UpdateFavoritesAsync(async () => await FavoriteStationsViewModel.ToggleAsync(e.Station)
            ? "Dodano do ulubionych"
            : "Usunięto z ulubionych");

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    private async void OnFavoriteStationRemoveRequested(object? sender, FavoriteStationEventArgs e)
        => await UpdateFavoritesAsync(async () => await FavoriteStationsViewModel.RemoveAsync(e.Station)
            ? "Usunięto z ulubionych"
            : "Stacji nie ma już w ulubionych");

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    private async void OnRetryFavoritesClicked(object? sender, EventArgs e)
        => await LoadFavoritesAsync();

    private async Task LoadFavoritesAsync()
    {
        StationLists.IsEnabled = false;
        RetryFavoritesButton.IsVisible = false;
        FavoriteFeedback.IsVisible = false;
        ++_feedbackVersion;

        try
        {
            await FavoriteStationsViewModel.LoadAsync();
            StationLists.IsEnabled = true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load favorite stations.");
            FavoriteFeedbackText.Text = "Nie udało się wczytać ulubionych.";
            RetryFavoritesButton.IsVisible = true;
            FavoriteFeedback.IsVisible = true;
        }
    }

    private async Task UpdateFavoritesAsync(Func<Task<string>> action)
    {
        try
        {
            await ShowFavoriteFeedbackAsync(await action());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to update favorite stations.");
            await ShowFavoriteFeedbackAsync("Nie udało się zapisać zmian w ulubionych.");
        }
    }

    private async Task ShowFavoriteFeedbackAsync(string message)
    {
        var version = ++_feedbackVersion;
        FavoriteFeedbackText.Text = message;
        RetryFavoritesButton.IsVisible = false;
        FavoriteFeedback.IsVisible = true;

        await Task.Delay(2500);

        if (version == _feedbackVersion)
            FavoriteFeedback.IsVisible = false;
    }
}
