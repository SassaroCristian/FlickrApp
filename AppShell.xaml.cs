using FlickrApp.Views;
using FlickrApp.Views.Search;

namespace FlickrApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(DiscoverPage), typeof(DiscoverPage));
        Routing.RegisterRoute(nameof(PhotoDetailsPage), typeof(PhotoDetailsPage));
        Routing.RegisterRoute(nameof(MapsPage), typeof(MapsPage));
        Routing.RegisterRoute(nameof(MapResultsPage), typeof(MapResultsPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(SearchResultPage), typeof(SearchResultPage));
        Routing.RegisterRoute(nameof(LikedPhotosPage), typeof(LikedPhotosPage));
    }
}