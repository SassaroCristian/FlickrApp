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
    private MapsViewModel _vm;

    public MapsPage(MapsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
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

        MyMap.Pins.Clear();
        MyMap.Pins.Add(pin);

        Debug.WriteLine($"Added pin at [Lat: {tappedLocation.Latitude:F5}, Lon: {tappedLocation.Longitude:F5}]");

        _ = _vm.AddPinToMap(tappedLocation.Latitude, tappedLocation.Longitude);
    }
}