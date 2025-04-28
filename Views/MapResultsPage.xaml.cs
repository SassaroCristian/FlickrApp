using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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