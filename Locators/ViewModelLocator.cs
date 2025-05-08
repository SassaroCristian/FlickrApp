using FlickrApp.ViewModels;

namespace FlickrApp.Locators;

public static class ViewModelLocator
{
    private static IServiceProvider? ServiceProvider { get; set; }

    public static AppShellViewModel AppShellViewModel => Resolve<AppShellViewModel>();
    public static DiscoverViewModel DiscoverViewModel => Resolve<DiscoverViewModel>();
    public static MapResultsViewModel MapResultsViewModel => Resolve<MapResultsViewModel>();
    public static MapsViewModel MapsViewModel => Resolve<MapsViewModel>();
    public static PhotoDetailsViewModel PhotoDetailsViewModel => Resolve<PhotoDetailsViewModel>();
    public static SearchViewModel SearchViewModel => Resolve<SearchViewModel>();
    public static LikedPhotosViewModel LikedPhotosViewModel => Resolve<LikedPhotosViewModel>();

    public static void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private static T Resolve<T>() where T : class
    {
        if (ServiceProvider == null)
            throw new InvalidOperationException("ViewModelLocator is not initialized. Call Initialize(...) first.");

        return ServiceProvider.GetRequiredService<T>();
    }
}