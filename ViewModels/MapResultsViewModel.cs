using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(Latitude), nameof(Latitude))]
[QueryProperty(nameof(Longitude), nameof(Longitude))]
public partial class MapResultsViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    [ObservableProperty] private string _latitude = string.Empty;
    [ObservableProperty] private string _longitude = string.Empty;

    private bool _isLatitudeSet = false;
    private bool _isLongitudeSet = false;

    private int _page = 1;
    private const int perPage = 10;

    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    public MapResultsViewModel()
    {
    }

    public MapResultsViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
    }


    partial void OnLatitudeChanged(string value)
    {
        _isLatitudeSet = true;
        TryLoadData();
    }

    partial void OnLongitudeChanged(string value)
    {
        _isLongitudeSet = true;
        TryLoadData();
    }

    private void TryLoadData()
    {
        Debug.WriteLine("try load data");
        if (_isLatitudeSet && _isLongitudeSet)
            Task.Run(FillDataAsync);
    }

    private async Task FillDataAsync()
    {
        Debug.WriteLine("filling data");
        try
        {
            _page = 1;
            Debug.WriteLine($"latitude: {Latitude}, longitude: {Longitude}");
            var result = await _flickr.GetForLocationAsync(Latitude, Longitude, _page, perPage);

            Photos = new ObservableCollection<FlickrPhoto>(result);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadMoreItemsAsync()
    {
        try
        {
            _page++;
            var result = await _flickr.GetMoreForLocationAsync(Latitude, Longitude, _page, perPage);
            foreach (var photo in result)
            {
                Photos.Add(photo);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    [RelayCommand]
    private async Task GoToPhotoDetailsAsync(FlickrPhoto photo)
    {
        await _navigation.GoToAsync("PhotoDetailsPage",
            new Dictionary<string, object>() { { "PhotoId", photo.Id } });
    }
}