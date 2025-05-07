using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public abstract partial class PhotoListViewModelBase(INavigationService navigation) : BaseViewModel
{
    private int _page = 1;
    private int _perPage = 20;

    [ObservableProperty] private bool _areMoreItemsAvailable = true;
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];

    protected async Task InitializeAsync(int perPage = 20)
    {
        AreMoreItemsAvailable = true;
        _page = 1;
        _perPage = perPage;

        await ExecuteSafelyAsync(async () =>
        {
            var photos = await FetchMoreItemsAsync(_page, perPage);
            Photos = new ObservableCollection<FlickrPhoto>(photos);

            if (photos.Count < _perPage) AreMoreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private async Task LoadMoreMoreItemsAsync()
    {
        if (!AreMoreItemsAvailable) return;

        await ExecuteSafelyAsync(async () =>
        {
            _page++;

            var photos = await FetchMoreItemsAsync(_page, _perPage);
            foreach (var photo in Photos) Photos.Add(photo);

            if (photos.Count < _perPage) AreMoreItemsAvailable = false;
        });
    }

    [RelayCommand]
    private async Task GoToPhotoDetailsAsync(FlickrPhoto photo)
    {
        await ExecuteSafelyAsync(async () =>
        {
            await navigation.GoToAsync("PhotoDetailsPage",
                new Dictionary<string, object> { { "PhotoId", photo.Id } });
        });
    }

    protected abstract Task<ICollection<FlickrPhoto>> FetchMoreItemsAsync(int page, int perPage);
}