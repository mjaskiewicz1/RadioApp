using System.Diagnostics.CodeAnalysis;

using RadioApp.ViewModels;

namespace RadioApp.Views;

public partial class MainPage
{
    private readonly MainViewModel _viewModel;
    public SearchViewModel SearchViewModel { get; }
    public MainPage(MainViewModel viewModel, SearchViewModel searchViewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        SearchViewModel = searchViewModel;
        BindingContext = viewModel;
    }

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecommendedAsync();
    }
}