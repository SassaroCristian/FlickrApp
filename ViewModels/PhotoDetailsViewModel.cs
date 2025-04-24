using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;
using Debug = System.Diagnostics.Debug;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(PhotoId), nameof(PhotoId))]
public partial class PhotoDetailsViewModel : ObservableObject
{
    private IFlickrApiService _flickr;

    [ObservableProperty] private string _photoId = string.Empty;
    [ObservableProperty] private FlickrDetails? _details = null;
    [ObservableProperty] private ObservableCollection<FlickrComment> _comments = [];

    public PhotoDetailsViewModel()
    {
    }

    public PhotoDetailsViewModel(IFlickrApiService flickr)
    {
        _flickr = flickr;
    }

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine("photoId changed: " + value + "");
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        try
        {
            Debug.WriteLine("Getting details");
            var details = await _flickr.GetDetailsAsync(PhotoId);
            Details = details;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }

        try
        {
            Debug.WriteLine("Getting comments");
            var comments = await _flickr.GetCommentsAsync(PhotoId);
            if (comments.Count == 0)
            {
                Debug.WriteLine("no comments");
                return;
            }

            Comments = new ObservableCollection<FlickrComment>(comments);

            Debug.WriteLine("Comments:\n");
            foreach (var comment in Comments) Debug.WriteLine(comment.Content);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}