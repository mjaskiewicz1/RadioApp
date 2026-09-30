using CommunityToolkit.Maui;

using Core.Data.Repositories;
using Core.Interfaces;
using Core.Services;

using LibVLCSharp.MAUI;

using Microsoft.Extensions.Logging;

using RadioApp.Extensions;
using RadioApp.Interfaces;
using RadioApp.Services;
using RadioApp.ViewModels;
using RadioApp.Views;

namespace RadioApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp(Action<IServiceCollection>? configurePlatformServices = null)
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseLibVLCSharp().UseMauiCommunityToolkit();
        builder.Services.AddSingleton<IRadioDirectoryService, RadioDirectoryService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<StationSearchViewModel>();
        builder.Services.AddSingleton<FavoriteStationsViewModel>();
        builder.Services.AddSingleton<IRadioPlaybackService, RadioPlaybackService>();
        builder.Services.AddScoped<IFavoriteStationRepository, FavoriteStationRepository>();
        builder.AddLocalDatabase();
        configurePlatformServices?.Invoke(builder.Services);
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}