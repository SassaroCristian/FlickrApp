using FlickrApp.ViewModels;

namespace FlickrApp.Views;

public partial class PhotoDetailsPage : ContentPage
{
    public PhotoDetailsPage(PhotoDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}