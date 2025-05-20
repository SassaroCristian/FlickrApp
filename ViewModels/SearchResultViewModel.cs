using System.Diagnostics;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Entities;
using FlickrApp.Models.Lookups;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(SearchParameters), nameof(SearchParameters))]
public partial class SearchResultViewModel(
    INavigationService navigation,
    IDeviceService device,
    IFlickrApiService flickr,
    IMapper mapper)
    : PhotoListViewModelBase(navigation, device)
{
    private readonly INavigationService _navigation = navigation;

    [ObservableProperty] private SearchNavigationParams? _searchParameters;

    partial void OnSearchParametersChanged(SearchNavigationParams? value)
    {
        _ = InitializeAsync();
    }

    protected override async Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage)
    {
        if (SearchParameters == null) return [];
        Debug.WriteLine($"---> Fetching more items for:" +
                        $"\n    Text: {SearchParameters.SearchText}" +
                        $"\n    Tags: {SearchParameters.SearchTags}" +
                        $"\n    License: {SearchParameters.LicenseId}" +
                        $"\n    ContentType: {SearchParameters.ContentType}" +
                        $"\n    GeoContext: {SearchParameters.GeoContext}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await flickr.SearchAsync(SearchParameters.SearchText, SearchParameters.SearchTags,
                SearchParameters.StartDate, SearchParameters.EndDate, SearchParameters.LicenseId,
                SearchParameters.ContentType, SearchParameters.GeoContext, page, perPage,
                SearchParameters.SortCriterionValue);
            var result = items.Select(mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }

    protected override async Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage)
    {
        if (SearchParameters == null) return [];
        Debug.WriteLine($"---> Fetching more items for:" +
                        $"\n    Text: {SearchParameters.SearchText}" +
                        $"\n    Tags: {SearchParameters.SearchTags}" +
                        $"\n    License: {SearchParameters.LicenseId}" +
                        $"\n    ContentType: {SearchParameters.ContentType}" +
                        $"\n    GeoContext: {SearchParameters.GeoContext}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await flickr.SearchMoreAsync(SearchParameters.SearchText, SearchParameters.SearchTags,
                SearchParameters.StartDate, SearchParameters.EndDate, SearchParameters.LicenseId,
                SearchParameters.ContentType, SearchParameters.GeoContext, page, perPage,
                SearchParameters.SortCriterionValue);
            var result = items.Select(mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }
}