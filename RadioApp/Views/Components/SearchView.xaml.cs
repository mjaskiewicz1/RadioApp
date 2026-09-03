using Core.Models;

using RadioApp.ViewModels;

namespace RadioApp.Views.Components;

public partial class SearchView : ContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(SearchViewModel), typeof(SearchView),
            propertyChanged: OnViewModelChanged);

    public static readonly BindableProperty SelectedStationProperty =
        BindableProperty.Create(nameof(SelectedStation), typeof(RadioStation), typeof(SearchView),
            defaultBindingMode: BindingMode.TwoWay);

    public SearchViewModel? ViewModel
    {
         get => (SearchViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public RadioStation? SelectedStation
    {
        get => (RadioStation?)GetValue(SelectedStationProperty);
        set => SetValue(SelectedStationProperty, value);
    }

    public SearchView()
    {
        InitializeComponent();
    }

    private static void OnViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchView searchView)
            searchView.SearchContent.BindingContext = newValue;
    }

    private void OnStationTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Grid { BindingContext: RadioStation station })
            SelectedStation = station;
    }
}