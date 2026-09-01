using System.Diagnostics.CodeAnalysis;

using RadioApp.ViewModels;

namespace RadioApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    [SuppressMessage("ReSharper", "AsyncVoidMethod")]
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecommendedAsync();
    }
}