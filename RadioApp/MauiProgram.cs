using Core.Data.Repositories;
using Core.Interfaces;
using Core.Services;

using LibVLCSharp.MAUI;

using Microsoft.Extensions.Logging;

using RadioApp.Extensions;
using RadioApp.ViewModels;
using RadioApp.Views;


namespace RadioApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("RemoveUnderline", (handler, _) => handler.PlatformView.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent));
#endif
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseLibVLCSharp();
        builder.Services.AddSingleton<IRadioDirectoryService, RadioDirectoryService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<StationSearchViewModel>();
        builder.Services.AddScoped<IFavoriteStationRepository, FavoriteStationRepository>();
        builder.AddLocalDatabase();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}