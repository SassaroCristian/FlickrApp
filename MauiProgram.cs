using CommunityToolkit.Maui;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Flickr.Net;
using Microsoft.Extensions.Logging;

namespace FlickrApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // SINGLETON
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<Flickr.Net.Flickr>(sp => new Flickr.Net.Flickr("255ac8fdac4726aa339fa1c2161b9e5b") );

        // TRANSIENT
        builder.Services.AddTransient<DiscoverViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}