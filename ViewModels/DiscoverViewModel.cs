using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    private int _currentPage = 1;
    private const int pageSize = 15;
    private bool _moreItemsAvailable = true;


    private const string excludedTags = "-naked,-Naked";
    
    [ObservableProperty] private string _title = "Discover";
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    [ObservableProperty] private bool _isLoading = false;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FilterDisplayTitle))]
    private string _currentTagFilter = string.Empty;

    public string FilterDisplayTitle
    {
        get
        {
            if (string.IsNullOrEmpty(CurrentTagFilter))
            {
                return "Popular:";
            }
            else
            {
                
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return $"{textInfo.ToTitleCase(CurrentTagFilter)}:";
            }
        }
    }

    public DiscoverViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
        InitializeViewModelCommand = new AsyncRelayCommand(InitializeViewModel); 
        InitializeViewModelCommand.Execute(null);
    }

    public IAsyncRelayCommand InitializeViewModelCommand { get; }

    private async Task InitializeViewModel()
    {
        
        await FetchPhotosAsync(page: 1, isNewSearchOrFilter: true, tag: string.Empty);
    }

    // private async Task LoadItems()
    // {
    //     try
    //     {
    //         var recentPhotos = await _flickr.GetRecentAsync(1, 10);
    //         Photos = new ObservableCollection<FlickrPhoto>(recentPhotos);
    //         page = 2;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.WriteLine(e.Message);
    //     }
    // }

    [RelayCommand(CanExecute = nameof(CanLoadMore))]
    private async Task LoadMoreItems()
    {
        // try
        // {
        //     var recentPhotos = await _flickr.GetMoreRecentAsync(page, 10);
        //     recentPhotos.ForEach(element => Photos.Add(element));
        //     page++;
        // }
        // catch (Exception e)
        // {
        //     Debug.WriteLine(e.Message);
        // }
        await FetchPhotosAsync(page: _currentPage + 1, isNewSearchOrFilter: false, tag: CurrentTagFilter);
    }

    private bool CanLoadMore() 
    {
        return !IsLoading && _moreItemsAvailable;
    }

    [RelayCommand]
    private async Task GoToPhotoDetails(FlickrPhoto photo)
    {
        if (photo == null) 
        {
            Debug.WriteLine("GoToPhotoDetails called with null photo.");
            return;
        }

        await _navigation.GoToAsync("PhotoDetailsPage", new Dictionary<string, object>() { { "PhotoId", photo.Id } });
    }

    [RelayCommand]
    private async Task FilterByTag(string tag) 
    {
        await FetchPhotosAsync(page: 1, isNewSearchOrFilter: true, tag: tag ?? string.Empty);
    }


    private async Task FetchPhotosAsync(int page, bool isNewSearchOrFilter, string tag)
    {
        if (IsLoading || (!isNewSearchOrFilter && !_moreItemsAvailable))
        {
            Debug.WriteLine($"---> Fetch skipped. IsLoading: {IsLoading}, MoreItemsAvailable: {_moreItemsAvailable}");
            return;
        }

        IsLoading = true;

        if (isNewSearchOrFilter)
        {
            _moreItemsAvailable = true;
            Debug.WriteLine("---> Resetting moreItemsAvailable to true for new search/filter.");
        }
        LoadMoreItemsCommand.NotifyCanExecuteChanged();

        try
        {
            if (isNewSearchOrFilter)
            {
                CurrentTagFilter = tag;
                _currentPage = 1;
                Photos.Clear();
                Debug.WriteLine(
                    $"---> New Search/Filter. Tag: '{CurrentTagFilter}'. Page reset to {_currentPage}.");
            }
            else
            {
                _currentPage = page;
                Debug.WriteLine($"---> Load More. Tag: '{CurrentTagFilter}'. Requesting page: {_currentPage}.");
            }

            
            string finalTagsParameter;
            if (!string.IsNullOrEmpty(CurrentTagFilter))
            {
                finalTagsParameter = $"{CurrentTagFilter},{excludedTags}";
            }
            else
            {
                finalTagsParameter = excludedTags;
            }
            Debug.WriteLine($"---> Using final tags parameter for API: '{finalTagsParameter}'");
            

            
            var newPhotos = await _flickr.SearchAsync(
                text: null,
                tags: finalTagsParameter, 
                page: _currentPage,
                perPage: pageSize
            );

            if (newPhotos != null && newPhotos.Any())
            {
                int addedCount = 0;
                foreach (var photo in newPhotos)
                {
                    if (!Photos.Any(p => p.Id == photo.Id))
                    {
                        Photos.Add(photo);
                        addedCount++;
                    }
                    else
                    {
                        Debug.WriteLine($"---> Skipping duplicate photo ID: {photo.Id}");
                    }
                }
                Debug.WriteLine($"---> Added {addedCount} NEW photos.");

                if (newPhotos.Count < pageSize)
                {
                    _moreItemsAvailable = false;
                    Debug.WriteLine(
                        "---> Received fewer photos than PageSize from API, assuming no more items available.");
                }
            }
            else
            {
                 Debug.WriteLine("---> No photos received from API.");
                 _moreItemsAvailable = false;
            }
        }
        catch (Exception ex)
        {
             Debug.WriteLine($"### ERROR during FetchPhotosAsync: {ex.Message}");
             _moreItemsAvailable = false;
        }
        finally
        {
            IsLoading = false;
            LoadMoreItemsCommand.NotifyCanExecuteChanged();
        }
    }
}