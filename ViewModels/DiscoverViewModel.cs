using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;
using FlickrApp.Models;

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
    private bool _isProgrammaticChangeNesting = false;

    [ObservableProperty] private string _filterDisplayTitle = "Popular";
    [ObservableProperty] private FlickrSortOption _selectedSortOption;
    [ObservableProperty] private ObservableCollection<SortOptionDisplay> _availableSortOptions;
    [ObservableProperty] private SortOptionDisplay? _selectedSortOptionItem;
    [ObservableProperty] private string _selectedSortOptionDisplayName = "Most Interesting";

    public DiscoverViewModel(INavigationService navigation, IFlickrApiService flickr, IMapper mapper) : base(navigation)
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

        _isProgrammaticChangeNesting = true;
        SelectedSortOption = FlickrSortOption.InterestingnessDesc;
        if (SelectedSortOptionItem == null && SelectedSortOption == FlickrSortOption.InterestingnessDesc)
        {
            SelectedSortOptionItem =
                _availableSortOptions.FirstOrDefault(o => o.SortEnumValue == FlickrSortOption.InterestingnessDesc);
        }

        _isProgrammaticChangeNesting = false;

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

            bool oldNestingState = _isProgrammaticChangeNesting;
            _isProgrammaticChangeNesting = true;
            SelectedSortOption = newValue.SortEnumValue;
            _isProgrammaticChangeNesting = oldNestingState;

            if (!_isProgrammaticChangeNesting)
            {
                Debug.WriteLine($"DiscoverViewModel: SelectedSortOptionItem changed by UI. Refreshing photos.");
                await InitializeAsync();
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
        _isProgrammaticChangeNesting = true;
        if (SelectedSortOption != newSortOption)
        {
            SelectedSortOption = newSortOption;
        }

        _isProgrammaticChangeNesting = false;

        Debug.WriteLine(
            $"DiscoverViewModel: SetSortOrderAsync command executed with {newSortOption}. Refreshing photos.");
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task FilterByTagAsync(string? tag)
    {
        _currentTagFilter = tag ?? string.Empty;
        FilterDisplayTitle = string.IsNullOrWhiteSpace(tag) ? "Popular" : tag[..1].ToUpper() + tag[1..].ToLower();

        _isProgrammaticChangeNesting = true;
        if (SelectedSortOption != FlickrSortOption.InterestingnessDesc)
        {
            SelectedSortOption = FlickrSortOption.InterestingnessDesc;
        }

        _isProgrammaticChangeNesting = false;

        await InitializeAsync();
    }

    private bool IsInitialized()
    {
        return Photos.Any();
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