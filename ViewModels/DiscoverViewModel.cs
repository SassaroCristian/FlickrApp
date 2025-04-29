using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;
using System.Linq;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IFlickrApiService _flickr;

    private int _currentPage = 1;
    private const int pageSize = 4;
    private bool moreItemsAvailable = true;


    [ObservableProperty] private string _title = "Discover";
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    // public DiscoverViewModel()
    // {
    // }

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
                // Capitalize the first letter of the tag for better display
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                return $"{textInfo.ToTitleCase(CurrentTagFilter)}:";
            }
        }
    }

    public DiscoverViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;
        // Task.Run(LoadItems);
        InitializeViewModelCommand = new AsyncRelayCommand(InitializeViewModel); // comando per l'inizializzazione
        InitializeViewModelCommand.Execute(null);
    }

    public IAsyncRelayCommand InitializeViewModelCommand { get; }

    private async Task InitializeViewModel()
    {
        // Carica i dati iniziali (popolari/recenti)
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

    private bool CanLoadMore() // Metodo che abilita/disabilita il comando
    {
        return !IsLoading && moreItemsAvailable;
    }

    [RelayCommand]
    private async Task GoToPhotoDetails(FlickrPhoto photo)
    {
        if (photo == null) // Add this check
        {
            Debug.WriteLine("GoToPhotoDetails called with null photo.");
            return;
        }

        await _navigation.GoToAsync("PhotoDetailsPage", new Dictionary<string, object>() { { "PhotoId", photo.Id } });
    }

    [RelayCommand]
    private async Task FilterByTag(string tag) // Il comando riceve il tag come stringa
    {
        await FetchPhotosAsync(page: 1, isNewSearchOrFilter: true, tag: tag ?? string.Empty);
    }


    private async Task FetchPhotosAsync(int page, bool isNewSearchOrFilter, string tag)
    {
        if (IsLoading || (!isNewSearchOrFilter && !moreItemsAvailable))
            return;

        IsLoading = true;
        
        if (isNewSearchOrFilter)
        {
            moreItemsAvailable = true;
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
                    $"---> Nuova Ricerca/Filtro. Tag: '{CurrentTagFilter}'. Pagina resettata a {_currentPage}.");
            }
            else
            {
                _currentPage = page;
                Debug.WriteLine($"---> Carica Altro. Tag: '{CurrentTagFilter}'. Pagina: {_currentPage}.");
            }


            var newPhotos = await _flickr.SearchAsync(
                text: null,
                tags: CurrentTagFilter,
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
                    moreItemsAvailable = false;
                    Debug.WriteLine(
                        "---> Received fewer photos than PageSize from API, assuming no more items available.");
                }
            }
            else
            {
                moreItemsAvailable = false;
            }
        }
        catch (Exception ex)
        {
            moreItemsAvailable = false;
        }
        finally
        {
            IsLoading = false;
            LoadMoreItemsCommand.NotifyCanExecuteChanged();
        }
    }
}