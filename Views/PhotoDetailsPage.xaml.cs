using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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