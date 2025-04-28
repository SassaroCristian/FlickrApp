using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;
using Debug = System.Diagnostics.Debug;

namespace FlickrApp.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    private int _page = 1;
    private const int perPage = 10;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    public SearchViewModel()
    {
    }

    public SearchViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
    }

    [RelayCommand]
    private async Task Search()
    {
        try
        {
            _page = 1;
            var response = await _flickr.SearchAsync(SearchText, string.Empty, _page, perPage);
            Photos = new ObservableCollection<FlickrPhoto>(response);
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
            _page++;
            var response = await _flickr.SearchMoreAsync(SearchText, string.Empty, _page, perPage);
            foreach (var photo in response)
                Photos.Add(photo);
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