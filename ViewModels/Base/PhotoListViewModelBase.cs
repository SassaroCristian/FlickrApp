using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Services;

namespace FlickrApp.ViewModels.Base;

public abstract partial class PhotoListViewModelBase(INavigationService navigation) : BaseViewModel
{
    private int _page = 1;
    private int _perPage = 20;

    [ObservableProperty] private bool _areMoreItemsAvailable = true;
    [ObservableProperty] private ObservableCollection<PhotoEntity> _photos = [];

    protected async Task InitializeAsync(int perPage)
    {
        Debug.WriteLine($" ---> Initializing PhotoListViewModelBase, PerPage {perPage}");

        AreMoreItemsAvailable = true;
        _page = 1;
        _perPage = perPage;

        await ExecuteSafelyAsync(async () =>
        {
            var photos = await FetchItemsAsync(_page, perPage);
            if (photos.Count < _perPage) AreMoreItemsAvailable = false;
            Photos = new ObservableCollection<PhotoEntity>(photos);
        });
    }

    protected async Task InitializeAsync()
    {
        var currentIdiom = DeviceInfo.Idiom;
        var perPage = currentIdiom == DeviceIdiom.Phone ? 10 :
            currentIdiom == DeviceIdiom.Tablet ? 21 : 0;
        await InitializeAsync(perPage);
    }

    [RelayCommand]
    private async Task LoadMoreItemsAsync()
    {
        Debug.WriteLine(" ---> Loading More Items");
        
        if (!AreMoreItemsAvailable) return;

        await ExecuteSafelyAsync(async () =>
        {
            _page++;

            var photos = await FetchMoreItemsAsync(_page, _perPage);
            if (photos.Count < _perPage) AreMoreItemsAvailable = false;
            foreach (var photo in photos) Photos.Add(photo);
        });
    }
    

    [RelayCommand]
    private async Task GoToPhotoDetailsAsync(PhotoEntity photo)
    {
        await ExecuteSafelyAsync(async () =>
        {
            await navigation.GoToAsync("PhotoDetailsPage",
                new Dictionary<string, object> { { "PhotoId", photo.Id } });
        });
    }

    protected abstract Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage);

    protected abstract Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage);
}