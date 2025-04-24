using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    private int page = 1;

    [ObservableProperty] private string _title = "Discover";
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    public DiscoverViewModel()
    {
    }

    public DiscoverViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
        Task.Run(LoadItems);
    }

    private async Task LoadItems()
    {
        try
        {
            var recentPhotos = await _flickr.GetRecentAsync(1, 10);
            Photos = new ObservableCollection<FlickrPhoto>(recentPhotos);
            page = 2;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    [RelayCommand]
    private async Task LoadMoreItems()
    {
        try
        {
            var recentPhotos = await _flickr.GetMoreRecentAsync(page, 10);
            recentPhotos.ForEach(element => Photos.Add(element));
            page++;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    [RelayCommand]
    private async Task GoToPhotoDetails(FlickrPhoto photo)
    {
        await _navigation.GoToAsync("PhotoDetailsPage", new Dictionary<string, object>() { { "PhotoId", photo.Id } });
    }
}