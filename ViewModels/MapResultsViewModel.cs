using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(Latitude), nameof(Latitude))]
[QueryProperty(nameof(Longitude), nameof(Longitude))]
public partial class MapResultsViewModel(INavigationService navigation, IFlickrApiService flickr)
    : PhotoListViewModelBase(navigation)
{
    private const int perPageInit = 10;

    private bool _isLatitudeSet;
    private bool _isLongitudeSet;

    [ObservableProperty] private string _latitude = string.Empty;
    [ObservableProperty] private string _longitude = string.Empty;
    
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
        Debug.WriteLine($" ---> Pin is at latitude: {Latitude}, longitude: {Longitude}");

        await InitializeAsync(perPageInit);
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchItemsAsync(int page, int perPage)
    {
        var items = await flickr.GetForLocationAsync(Latitude, Longitude, page, perPage);
        return items;
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchMoreItemsAsync(int page, int perPage)
    {
        await ExecuteSafelyAsync(async () =>
        {
            var items = await flickr.GetMoreForLocationAsync(Latitude, Longitude, page, perPage);
            return items;
        });
        return [];
    }
}