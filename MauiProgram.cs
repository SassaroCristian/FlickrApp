using CommunityToolkit.Maui;
using FlickrApp.Locators;
using FlickrApp.Services;
using Microsoft.Maui.Controls;
using FlickrApp.ViewModels;
using FlickrApp.Views;
using Microsoft.Extensions.Logging;
using FlickrApp.Repositories;
using SQLite;
using System.IO;
using FlickrApp.Entities;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Hosting;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif


        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IFlickrApiService, FlickrApiService>();

        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Photos.db3");
            return new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
            );
        });

        builder.Services.AddSingleton<IPhotoRepository, PhotoRepository>();
        builder.Services.AddSingleton<ILocalFileSystemService, LocalFileSystemService>();

        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<AppShellViewModel>();
        builder.Services.AddTransient<DiscoverPage>();
        builder.Services.AddTransient<DiscoverViewModel>();
        builder.Services.AddTransient<PhotoDetailsPage>();
        builder.Services.AddTransient<PhotoDetailsViewModel>();
        builder.Services.AddTransient<MapsPage>();
        builder.Services.AddTransient<MapsViewModel>();
        builder.Services.AddTransient<MapResultsPage>();
        builder.Services.AddTransient<MapResultsViewModel>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<SearchViewModel>();

        var app = builder.Build();

        var conn = app.Services.GetRequiredService<SQLiteAsyncConnection>();
        conn.CreateTableAsync<Photo>()
            .GetAwaiter()
            .GetResult();

        ViewModelLocator.Initialize(app.Services);

        return app;
    }
}
