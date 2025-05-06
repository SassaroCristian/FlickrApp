using System.ComponentModel;
using System.Diagnostics;
using FlickrApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace FlickrApp.Views;

public partial class MapsPage : ContentPage
{
    private readonly MapsViewModel? _vm;
    
    public MapsPage(MapsViewModel vm)
    {
        InitializeComponent();

        _vm = BindingContext as MapsViewModel;
        if (_vm == null)
            return;
        
        _vm.PropertyChanged += ViewModel_PropertyChanged;
    }


    private async void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_vm == null) return;
        if (e.PropertyName != nameof(MapsViewModel.SelectedWonder)) return;
        if (_vm.SelectedWonder == null) return;

        SetMapLocation(_vm.SelectedWonder.Location);

        var wonderPin = new Pin
        {
            Label = _vm.SelectedWonder.Name,
            Location = _vm.SelectedWonder.Location,
            Type = PinType.Place
        };
        MyMap.Pins.Add(wonderPin);

        await _vm.AddPinToMap(_vm.SelectedWonder.Location.Latitude, _vm.SelectedWonder.Location.Longitude);
    }


    private void SetMapLocation(Location location) 
    {
        const double latitudeDegrees = 0.1;
        const double longitudeDegrees = 0.1;

        var span = new MapSpan(location, latitudeDegrees, longitudeDegrees);

        MyMap.Pins.Clear();
        
        MyMap.MoveToRegion(span);

        Debug.WriteLine($"Mappa centrata su: {location.Latitude:F5}, Lon: {location.Longitude:F5}");

        MyMap.Pins.Clear();
    }


    private async void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        try
        {
            if (_vm == null) return;

            var tappedLocation = e.Location;

            MyMap.Pins.Clear();

            var pin = new Pin
            {
                Label = "Selected Location",
                Location = tappedLocation,
                Address = $"Lat: {tappedLocation.Latitude:F5}, Lon: {tappedLocation.Longitude:F5}",
                Type = PinType.Place
            };

            MyMap.Pins.Add(pin);

            Debug.WriteLine($"Added pin at [Lat: {tappedLocation.Latitude:F5}, Lon: {tappedLocation.Longitude:F5}]");

            await _vm.AddPinToMap(tappedLocation.Latitude, tappedLocation.Longitude);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_vm != null)
        {
            _vm.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }
}
