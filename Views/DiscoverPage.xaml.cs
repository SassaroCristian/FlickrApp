using FlickrApp.ViewModels;

namespace FlickrApp.Views;

public partial class DiscoverPage : ContentPage
{
    public DiscoverPage(DiscoverViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}