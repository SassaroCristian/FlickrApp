using CommunityToolkit.Maui;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using FlickrApp.Views;
using Microsoft.Extensions.Logging;

namespace FlickrApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
                fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");
                fonts.AddFont("Roboto-Italic.ttf", "RobotoItalic");
                fonts.AddFont("Roboto-BoldItalic.ttf", "RobotoItalicBold");
            });

        // SINGLETON
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IFlickrApiService, FlickrApiService>();

        // TRANSIENT
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<AppShellViewModel>();
        // DiscoverPage
        builder.Services.AddTransient<DiscoverPage>();
        builder.Services.AddTransient<DiscoverViewModel>();
        // PhotoDetails
        builder.Services.AddTransient<PhotoDetailsPage>();
        builder.Services.AddTransient<PhotoDetailsViewModel>();
        // Maps
        builder.Services.AddTransient<MapsPage>();
        builder.Services.AddTransient<MapsViewModel>();
        // Map Result
        builder.Services.AddTransient<MapResultsPage>();
        builder.Services.AddTransient<MapResultsViewModel>();
        // Search
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<SearchViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}