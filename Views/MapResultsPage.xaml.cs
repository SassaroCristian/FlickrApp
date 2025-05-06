using FlickrApp.ViewModels;

namespace FlickrApp.Views;

public partial class MapResultsPage : ContentPage
{
    public MapResultsPage(MapResultsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}