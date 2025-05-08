using System.Diagnostics;
using CommunityToolkit.Maui;
using FlickrApp.Entities;
using FlickrApp.Locators;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using FlickrApp.Views;
using Microsoft.Extensions.Logging;
using SQLite;

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

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            Debug.WriteLine($"AppDomain Unhandled Exception: {ex}");
        };
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Debug.WriteLine($"Unobserved Task Exception: {args.Exception}");
            args.SetObserved();
        };
        
        // SINGLETON
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
        // LikedPhoto
        builder.Services.AddTransient<LikedPhotosPage>();
        builder.Services.AddTransient<LikedPhotosViewModel>();

        var app = builder.Build();

        var conn = app.Services.GetRequiredService<SQLiteAsyncConnection>();
        conn.CreateTableAsync<Photo>()
            .GetAwaiter()
            .GetResult();

        ViewModelLocator.Initialize(app.Services);

        return app;
    }
}