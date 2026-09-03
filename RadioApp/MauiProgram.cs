using Core.Interfaces;
using Core.Services;

using LibVLCSharp.MAUI;

using Microsoft.Extensions.Logging;

using RadioApp.ViewModels;
using RadioApp.Views;


namespace RadioApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseLibVLCSharp();
        builder.Services.AddSingleton<IRadioDirectoryService, RadioDirectoryService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<SearchViewModel>();
        builder.Services.AddSingleton<MainPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}