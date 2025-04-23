using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Flickr.Net;

namespace FlickrApp.ViewModels;

public partial class DiscoverViewModel : ObservableObject
{
    private Flickr _flickr;

    [ObservableProperty] private ObservableCollection<Photo> _photos = [];

    public DiscoverViewModel(Flickr flickr)
    {
        _flickr = flickr;
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        var recentPhotos = await _flickr.PhotosGetRecentAsync(0, 10);
        Photos = new ObservableCollection<Photo>(recentPhotos);
        foreach (var photo in Photos)
        {
            Debug.WriteLine(photo.Title);
            Debug.WriteLine(photo.MediumUrl);
        }
    }
}