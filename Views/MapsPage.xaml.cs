using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlickrApp.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace FlickrApp.Views;

public partial class MapsPage : ContentPage
{
    public MapsPage(MapsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        var tappedLocation = e.Location;

        var pin = new Pin()
        {
            Label = "Selected Location",
            Location = tappedLocation,
            Address = $"Lat: {tappedLocation.Latitude:F5}, Lon: {tappedLocation.Longitude:F5}",
            Type = PinType.Place
        };

        myMap.Pins.Clear();
        myMap.Pins.Add(pin);

        Debug.WriteLine($"Added pin at [Lat: {tappedLocation.Latitude:F5}, Lon: {tappedLocation.Longitude:F5}]");
    }
}