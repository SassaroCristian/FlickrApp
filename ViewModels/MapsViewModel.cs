using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class MapsViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    [ObservableProperty] private bool _isPinned = false;
    [ObservableProperty] private bool _isListFull = false;
    [ObservableProperty] private string? _location = string.Empty;
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    private double _longitude = 0;
    private double _latitude = 0;


    public MapsViewModel()
    {
    }

    public MapsViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
    }

    public async Task AddPinToMap(double latitude, double longitude)
    {
        IsListFull = false;
        IsPinned = true;

        _latitude = latitude;
        _longitude = longitude;

        var locations = await Geocoding.GetPlacemarksAsync(latitude, longitude);
        Location = locations.FirstOrDefault()?.CountryName;

        await LoadItemsAsync(latitude, longitude);
        if (Photos.Count > 0) IsListFull = true;
    }

    private async Task LoadItemsAsync(double latitude, double longitude)
    {
        var photos =
            await _flickr.GetForLocationAsync(latitude.ToString(CultureInfo.InvariantCulture),
                longitude.ToString(CultureInfo.InvariantCulture), 1, 4);
        Photos = new ObservableCollection<FlickrPhoto>(photos);
    }

    [RelayCommand]
    private async Task GoToMapResultsAsync()
    {
        await _navigation.GoToAsync("MapResultsPage", new Dictionary<string, object>()
        {
            { "Latitude", _latitude.ToString(CultureInfo.InvariantCulture) },
            { "Longitude", _longitude.ToString(CultureInfo.InvariantCulture) },
        });
    }
}