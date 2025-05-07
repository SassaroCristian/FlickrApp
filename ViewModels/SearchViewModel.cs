using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public partial class SearchViewModel(INavigationService navigation, IFlickrApiService flickr)
    : PhotoListViewModelBase(navigation)
{
    private const int perPageInit = 10;
    
    [ObservableProperty] private string _searchText = string.Empty;

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Photos.Clear();
            return;
        }

        await InitializeAsync(perPageInit);
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($" ---> Fetching items for {SearchText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await flickr.SearchAsync(SearchText, string.Empty, page, perPage);
            return items;
        }) ?? [];
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchMoreItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($" ---> Fetching more items for {SearchText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await flickr.SearchMoreAsync(SearchText, string.Empty, page, perPage);
            return items;
        }) ?? [];
    }
}