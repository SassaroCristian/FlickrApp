using FlickrApp.ViewModels;
using FlickrApp.Views;

namespace FlickrApp;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;

        Routing.RegisterRoute(nameof(DiscoverPage), typeof(DiscoverPage));
        Routing.RegisterRoute(nameof(PhotoDetailsPage), typeof(PhotoDetailsPage));
        Routing.RegisterRoute(nameof(MapsPage), typeof(MapsPage));
        Routing.RegisterRoute(nameof(MapResultsPage), typeof(MapResultsPage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
    }
}