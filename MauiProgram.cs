using System.Diagnostics;
using CommunityToolkit.Maui;
using FlickrApp.Entities;
using FlickrApp.Locators;
using FlickrApp.Mappings;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using FlickrApp.Views;
using FlickrApp.Views.Search;
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
        builder.Services.AddSingleton<IFlickrApiService, FlickrApiService>();
        builder.Services.AddSingleton<IDeviceService, DeviceService>();
        builder.Services.AddSingleton<IFileSystemOperations, FileSystemOperations>();
        builder.Services.AddSingleton<IShellNavigation, ShellNavigation>();

        // SQLITE CONNECTIONC
        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Photos.db3");
            return new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
            );
        });

        // PHOTO REPO
        builder.Services.AddSingleton<IPhotoRepository, PhotoRepository>();
        builder.Services.AddSingleton<ILocalFileSystemService, LocalFileSystemService>();

        // MAPPER
        builder.Services.AddAutoMapper(typeof(PhotoProfile).Assembly, typeof(DetailProfile).Assembly);

        // GLOBAL ERROR HANDLER
        builder.Services.AddSingleton<IGlobalErrorHandler, GlobalErrorHandler>();

        // App Shell
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
        // Search Result
        builder.Services.AddTransient<SearchResultPage>();
        builder.Services.AddTransient<SearchResultViewModel>();
        // Liked Photos
        builder.Services.AddTransient<LikedPhotosPage>();
        builder.Services.AddTransient<LikedPhotosViewModel>();

        var app = builder.Build();
        
        var conn = app.Services.GetRequiredService<SQLiteAsyncConnection>();
        conn.CreateTableAsync<PhotoEntity>()
            .GetAwaiter()
            .GetResult();
        conn.CreateTableAsync<DetailEntity>()
            .GetAwaiter()
            .GetResult();

        ViewModelLocator.Initialize(app.Services);

        return app;
    }
}