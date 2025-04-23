using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Flickr.Net;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private Flickr.Net.Flickr _flickr;

    [ObservableProperty] private string _title = "Discover";
    [ObservableProperty] private ObservableCollection<Photo> _photos;

    public DiscoverViewModel()
    {
    }

    public DiscoverViewModel(Flickr.Net.Flickr flickr)
    {
        _flickr = flickr;
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        var recentPhotos = await _flickr.Photos.GetRecentAsync(0, 10);
        Photos = new ObservableCollection<Photo>(recentPhotos);
    }
}