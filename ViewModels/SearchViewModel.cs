using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public partial class SearchViewModel : PhotoListViewModelBase
{
    private INavigationService _navigation;
    private readonly IFlickrApiService _flickr;
    private readonly IMapper _mapper;
    
    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private ObservableCollection<FlickrLicense> _licensePickerList =
    [
        new() { Id = -1, Name = "No Licenses Selected", Url = string.Empty }
    ];

    [ObservableProperty] private FlickrLicense? _selectedLicense;

    public SearchViewModel(INavigationService navigation, IFlickrApiService flickr, IMapper mapper) : base(navigation)
    {
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
            var items = await _flickr.GetLicensesAsync();
            foreach (var item in items.OrderBy(i => i.Id))
                LicensePickerList.Add(item);
        });
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Photos.Clear();
            return;
        }

        await InitializeAsync();
    }

    protected override async Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($" ---> Fetching items for {SearchText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchAsync(SearchText, string.Empty, page, perPage);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }

    protected override async Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage)
    {
        Debug.WriteLine($" ---> Fetching more items for {SearchText}");
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _flickr.SearchMoreAsync(SearchText, string.Empty, page, perPage);
            var result = items.Select(_mapper.Map<PhotoEntity>).ToList();
            return result;
        }) ?? [];
    }
}