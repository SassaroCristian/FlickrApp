using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : PhotoListViewModelBase
{
    private const string excludedTags = "-naked,-Naked";
    private const int perPageInit = 15;
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;
    
    private string _currentTagFilter = string.Empty;

    [ObservableProperty] private string _filterDisplayTitle = "Popular";

    public DiscoverViewModel(INavigationService navigation, IFlickrApiService flickr) : base(navigation)
    {
        _navigation = navigation;
        _flickr = flickr;

        _ = InitializeAsync(perPageInit);
    }

    [RelayCommand]
    private async Task FilterByTagAsync(string? tag)
    {
        _currentTagFilter = tag ?? string.Empty;

        if (string.IsNullOrWhiteSpace(tag)) FilterDisplayTitle = "Popular";
        else FilterDisplayTitle = tag[..1].ToUpper() + tag[1..].ToLower();

        await InitializeAsync(perPageInit);
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchItemsAsync(int page, int perPage)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchAsync(string.Empty,
                string.Concat(_currentTagFilter, ",", excludedTags), page, perPage);
            return items;
        }) ?? [];
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchMoreItemsAsync(int page, int perPage)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchMoreAsync(string.Empty,
                string.Concat(_currentTagFilter, ",", excludedTags), page, perPage);
            return items;
        }) ?? [];
    }
}