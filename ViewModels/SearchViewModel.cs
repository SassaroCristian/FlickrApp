using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Models.Lookups;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public record PickerItem(int Value, string DisplayText);

public partial class SearchViewModel : PhotoListViewModelBase
{
    private static readonly DeviceIdiom CurrentIdiom = DeviceInfo.Idiom; 

    private INavigationService _navigation;
    private readonly IFlickrApiService _flickr;
    private readonly IMapper _mapper;

    [ObservableProperty] private List<SortCriterion> _sortCriteria = SortOptions.All;
    [ObservableProperty] private ObservableCollection<FlickrLicense> _licensePickerList = [];

    [ObservableProperty] private List<PickerItem> _contentTypePickerList =
    [
        new(0, "Photos"),
        new(1, "Screenshots"),
        new(2, "Other"),
        new(3, "Virtual Photos")
    ];

    [ObservableProperty] private List<PickerItem> _geoContextPickerList =
    [
        new(0, "Not Defined"),
        new(1, "Indoors"),
        new(2, "Outdoors")
    ];

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private SortCriterion _selectedSortCriterion = SortOptions.Relevance;
    [ObservableProperty] private string _searchTags = string.Empty;
    [ObservableProperty] private DateTime _startDate = DateTime.MinValue;
    [ObservableProperty] private DateTime _endDate = DateTime.UtcNow;
    [ObservableProperty] private DateTime _maxDate = DateTime.UtcNow;
    [ObservableProperty] private FlickrLicense? _selectedLicense;
    [ObservableProperty] private PickerItem? _selectedContentType;
    [ObservableProperty] private PickerItem? _selectedGeoContext;

    [ObservableProperty] private bool _isFilterChanged;

    public SearchViewModel(INavigationService navigation, IFlickrApiService flickr, IMapper mapper) : base(navigation)
    {
        Debug.WriteLine("---> starting search viewmodel");
        
        _navigation = navigation;
        _flickr = flickr;
        _mapper = mapper;

        _ = InitializeViewModelAsync();
    }

    private async Task InitializeViewModelAsync()
    {
        await ExecuteSafelyAsync(async () => { await LoadLicensesAsync(); });
    }

    private async Task LoadLicensesAsync()
    {
        await ExecuteSafelyAsync(async () =>
        {
            LicensePickerList.Add(new FlickrLicense { Id = -1, Name = "No Licenses", Url = string.Empty });
            
            var items = await _flickr.GetLicensesAsync();
            foreach (var item in items.OrderBy(i => i.Id))
                LicensePickerList.Add(item);
        });
    }

    [RelayCommand]
    private void Clear()
    {
        SearchText = string.Empty;
        SelectedSortCriterion = SortOptions.Relevance;
        SearchTags = string.Empty;
        StartDate = DateTime.MinValue;
        EndDate = DateTime.UtcNow;
        SelectedLicense = null;
        SelectedContentType = null;
        SelectedGeoContext = null;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (CurrentIdiom == DeviceIdiom.Tablet)
        {
            if (!IsFilterChanged) return;
            Photos.Clear();
            await InitializeAsync();
            IsFilterChanged = false;
        }
        else if (CurrentIdiom == DeviceIdiom.Phone)
        {
            var searchParams = new SearchNavigationParams
            {
                SearchText = SearchText,
                SearchTags = SearchTags,
                StartDate = StartDate,
                EndDate = EndDate,
                LicenseId = SelectedLicense?.Id != -1 ? SelectedLicense?.Id.ToString() : string.Empty,
                ContentType = SelectedContentType?.Value.ToString(),
                SortCriterionValue = SelectedSortCriterion.Value
            };
            await ExecuteSafelyAsync(async () =>
            {
                await _navigation.GoToAsync("SearchResultPage", new Dictionary<string, object>
                {
                    { "SearchParameters", searchParams }
                });
            });
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        IsFilterChanged = true;
    }

    partial void OnSelectedSortCriterionChanged(SortCriterion? value)
    {
        IsFilterChanged = true;
    }

    partial void OnSearchTagsChanged(string value)
    {
        IsFilterChanged = true;
    }

    partial void OnSelectedLicenseChanged(FlickrLicense? value)
    {
        IsFilterChanged = true;
    }

    partial void OnSelectedContentTypeChanged(PickerItem? value)
    {
        IsFilterChanged = true;
    }

    partial void OnSelectedGeoContextChanged(PickerItem? value)
    {
        IsFilterChanged = true;
    }

    protected override async Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($"---> Fetching more items for:" +
                        $"\n    Text: {SearchText}" +
                        $"\n    Tags: {SearchTags}" +
                        $"\n    License: {SelectedLicense?.Name}" +
                        $"\n    ContentType: {SelectedContentType?.DisplayText}" +
                        $"\n    GeoContext: {SelectedGeoContext?.DisplayText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchAsync(SearchText, SearchTags, StartDate, EndDate,
                SelectedLicense?.Id != -1 ? SelectedLicense?.Id.ToString() : string.Empty,
                SelectedContentType?.Value.ToString(),
                SelectedGeoContext?.Value.ToString(),
                page, perPage, SelectedSortCriterion.Value);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }

    protected override async Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($"---> Fetching more items for:" +
                        $"\n    Text: {SearchText}" +
                        $"\n    Tags: {SearchTags}" +
                        $"\n    License: {SelectedLicense?.Name}" +
                        $"\n    ContentType: {SelectedContentType?.DisplayText}" +
                        $"\n    GeoContext: {SelectedGeoContext?.DisplayText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchMoreAsync(SearchText, SearchTags, StartDate, EndDate,
                SelectedLicense?.Id != -1 ? SelectedLicense?.Id.ToString() : string.Empty,
                SelectedContentType?.Value.ToString(),
                SelectedGeoContext?.Value.ToString(),
                page, perPage, SelectedSortCriterion.Value);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }
}