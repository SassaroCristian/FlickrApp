using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(Latitude), nameof(Latitude))]
[QueryProperty(nameof(Longitude), nameof(Longitude))]
public partial class MapResultsViewModel(INavigationService navigation, IFlickrApiService flickr) : BaseViewModel
{
    private const int perPage = 10;

    private bool _isLatitudeSet;
    private bool _isLongitudeSet;

    private bool _areMoreItemsAvailable = true;

    [ObservableProperty] private string _latitude = string.Empty;
    [ObservableProperty] private string _longitude = string.Empty;

    private int _page = 1;

    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];


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
        Debug.WriteLine(" ---> try loading data");
        if (_isLatitudeSet && _isLongitudeSet)
            Task.Run(FillDataAsync);
    }

    private async Task FillDataAsync()
    {
        Debug.WriteLine("  ---> filling data");
        Debug.WriteLine($"---> Pin is at latitude: {Latitude}, longitude: {Longitude}");

        await ExecuteSafelyAsync(async () =>
        {
            _page = 1;
            _areMoreItemsAvailable = true;

            var result = await flickr.GetForLocationAsync(Latitude, Longitude, _page);
            Photos = new ObservableCollection<FlickrPhoto>(result);

            if (Photos.Count == perPage) _areMoreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private async Task LoadMoreItemsAsync()
    {
        if (!_areMoreItemsAvailable) return;
        Debug.WriteLine("  ---> loading more items");

        await ExecuteSafelyAsync(async () =>
        {
            _page++;

            var result = await flickr.GetMoreForLocationAsync(Latitude, Longitude, _page);
            foreach (var photo in result) Photos.Add(photo);

            if (result.Count == perPage) _areMoreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private async Task GoToPhotoDetailsAsync(FlickrPhoto photo)
    {
        await ExecuteSafelyAsync(async () =>
            await navigation.GoToAsync("PhotoDetailsPage", new Dictionary<string, object> { { "PhotoId", photo.Id } }));
    }
}