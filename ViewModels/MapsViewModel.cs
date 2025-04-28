using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class MapsViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    [ObservableProperty] private bool _isPinned = false;
    [ObservableProperty] private string? _location = string.Empty;
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

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
        IsPinned = true;
        var locations = await Geocoding.GetPlacemarksAsync(latitude, longitude);
        Location = locations.FirstOrDefault()?.CountryName;

        await LoadItemsAsync(latitude, longitude);
    }

    private async Task LoadItemsAsync(double latitude, double longitude)
    {
        var photos = await _flickr.GetForLocationAsync(latitude, longitude, 1, 4);
        Photos = new ObservableCollection<FlickrPhoto>(photos);
    }
}