using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class SearchViewModel(INavigationService navigation, IFlickrApiService flickr) : BaseViewModel
{
    private const int perPage = 10;

    private bool _moreItemsAvailable = true;
    private int _page = 1;
    
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];
    [ObservableProperty] private string _searchText = string.Empty;

    [RelayCommand]
    private void Search()
    {
        _ = ExecuteSafelyAsync(async () =>
        {
            _page = 1;
            _moreItemsAvailable = true;

            var response = await flickr.SearchAsync(SearchText, string.Empty, _page);
            Photos = new ObservableCollection<FlickrPhoto>(response);

            if (response.Count < perPage) _moreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private void LoadMoreItems()
    {
        if (!_moreItemsAvailable) return;

        _ = ExecuteSafelyAsync(async () =>
        {
            _page++;

            var response = await flickr.SearchMoreAsync(SearchText, string.Empty, _page);
            foreach (var photo in response)
                Photos.Add(photo);

            if (response.Count < perPage) _moreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private async Task GoToPhotoDetails(FlickrPhoto photo)
    {
        await navigation.GoToAsync("PhotoDetailsPage", new Dictionary<string, object> { { "PhotoId", photo.Id } });
    }
}