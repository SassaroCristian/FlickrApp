using FlickrApp.Views;

namespace FlickrApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(DiscoverPage), typeof(DiscoverPage));
        Routing.RegisterRoute(nameof(PhotoDetailsPage), typeof(PhotoDetailsPage));
        Routing.RegisterRoute(nameof(MapsPage), typeof(MapsPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
    }
}