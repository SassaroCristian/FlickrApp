using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrNet;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private Flickr _flickr;

    [ObservableProperty] private string _title = "Discover";
    [ObservableProperty] private ObservableCollection<Photo> _photos;

    public DiscoverViewModel()
    {
    }

    public DiscoverViewModel(Flickr flickr)
    {
        _flickr = flickr;
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        var recentPhotos = await _flickr.PhotosGetRecentAsync(0, 10);
        Photos = new ObservableCollection<Photo>(recentPhotos);
    }
}