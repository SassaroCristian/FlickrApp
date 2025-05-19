using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public enum FlickrSortOption
{
    InterestingnessDesc,
    InterestingnessAsc,
    DatePostedDesc,
    DatePostedAsc
}

public record SortOptionDisplay(FlickrSortOption SortEnumValue, string DisplayName);

public partial class DiscoverViewModel : PhotoListViewModelBase
{
    private const string excludedTags = "-naked,-Naked";
    private readonly IFlickrApiService _flickr;
    private readonly IMapper _mapper;

    private string _currentTagFilter = string.Empty;

    [ObservableProperty] private string _filterDisplayTitle = "Popular";

    [ObservableProperty] private FlickrSortOption _selectedSortOption;

    [ObservableProperty] private ObservableCollection<SortOptionDisplay> _availableSortOptions;

    [ObservableProperty] private SortOptionDisplay? _selectedSortOptionItem;

    [ObservableProperty] private string _selectedSortOptionDisplayName = string.Empty;

    public DiscoverViewModel(INavigationService navigation, IDeviceService device, IFlickrApiService flickr,
        IMapper mapper) : base(navigation, device)
    {
        _flickr = flickr;
        _mapper = mapper;

        _availableSortOptions = new ObservableCollection<SortOptionDisplay>
        {
            new(FlickrSortOption.InterestingnessDesc, "Most Interesting"),
            new(FlickrSortOption.InterestingnessAsc, "Least Interesting"),
            new(FlickrSortOption.DatePostedDesc, "Newest First"),
            new(FlickrSortOption.DatePostedAsc, "Oldest First")
        };

        SelectedSortOption = FlickrSortOption.InterestingnessDesc;

        _ = InitializeAsync();
    }

    partial void OnSelectedSortOptionChanged(FlickrSortOption value)
    {
        SelectedSortOptionDisplayName =
            AvailableSortOptions.FirstOrDefault(o => o.SortEnumValue == value)?.DisplayName ?? "N/A";

        if (SelectedSortOptionItem == null || SelectedSortOptionItem.SortEnumValue != value)
        {
            SelectedSortOptionItem = AvailableSortOptions.FirstOrDefault(o => o.SortEnumValue == value);
        }

        Debug.WriteLine(
            $"DiscoverViewModel: SelectedSortOption (enum) changed to: {value}, DisplayName: {SelectedSortOptionDisplayName}");
    }

    async partial void OnSelectedSortOptionItemChanged(SortOptionDisplay? oldValue, SortOptionDisplay? newValue)
    {
        if (newValue != null && (oldValue == null || oldValue.SortEnumValue != newValue.SortEnumValue))
        {
            Debug.WriteLine($"DiscoverViewModel: SelectedSortOptionItem (Picker) changed to: {newValue.DisplayName}");
            SelectedSortOption = newValue.SortEnumValue;
            if (oldValue?.SortEnumValue != newValue.SortEnumValue)
            {
                await SetSortOrderAsync(newValue.SortEnumValue);
            }
        }
        else if (newValue == null && oldValue != null)
        {
            Debug.WriteLine($"DiscoverViewModel: SelectedSortOptionItem (Picker) deselected.");
        }
    }

    private string GetSortOrderApiString(FlickrSortOption sortOption)
    {
        return sortOption switch
        {
            FlickrSortOption.InterestingnessDesc => "interestingness-desc",
            FlickrSortOption.InterestingnessAsc => "interestingness-asc",
            FlickrSortOption.DatePostedDesc => "date-posted-desc",
            FlickrSortOption.DatePostedAsc => "date-posted-asc",
            _ => "interestingness-desc"
        };
    }

    [RelayCommand]
    private async Task SetSortOrderAsync(FlickrSortOption newSortOption)
    {
        if (SelectedSortOption != newSortOption)
        {
            SelectedSortOption = newSortOption;
        }

        Debug.WriteLine($"DiscoverViewModel: SetSortOrderAsync called with {SelectedSortOption}. Refreshing photos.");
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task FilterByTagAsync(string? tag)
    {
        _currentTagFilter = tag ?? string.Empty;
        FilterDisplayTitle = string.IsNullOrWhiteSpace(tag) ? "Popular" : tag[..1].ToUpper() + tag[1..].ToLower();

        SelectedSortOption = FlickrSortOption.InterestingnessDesc;

        await InitializeAsync();
    }

    protected override async Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var apiSortOrder = GetSortOrderApiString(SelectedSortOption);

            var effectiveTags = !string.IsNullOrEmpty(_currentTagFilter)
                ? string.Concat(_currentTagFilter, ",", excludedTags)
                : excludedTags;

            Debug.WriteLine(
                $"DiscoverViewModel.FetchItemsAsync: Page: {page}, PerPage: {perPage}, Sort: {apiSortOrder}, Tags: '{effectiveTags}'");
            var items = await _flickr.SearchAsync(tags: effectiveTags, page: page, perPage: perPage,
                sortOrder: apiSortOrder);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }

    protected override async Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var apiSortOrder = GetSortOrderApiString(SelectedSortOption);

            var effectiveTags = !string.IsNullOrEmpty(_currentTagFilter)
                ? string.Concat(_currentTagFilter, ",", excludedTags)
                : excludedTags;

            Debug.WriteLine(
                $"DiscoverViewModel.FetchMoreItemsAsync: Page: {page}, PerPage: {perPage}, Sort: {apiSortOrder}, Tags: '{effectiveTags}'");
            var items = await _flickr.SearchMoreAsync(tags: effectiveTags, page: page, perPage: perPage,
                sortOrder: apiSortOrder);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }
}