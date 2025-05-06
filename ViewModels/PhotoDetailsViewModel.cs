using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;
using Debug = System.Diagnostics.Debug;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(PhotoId), nameof(PhotoId))]
public partial class PhotoDetailsViewModel : ObservableObject
{
    private readonly IFlickrApiService _flickr;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CommentsHeaderTitle))]
    private ObservableCollection<FlickrComment> _comments = [];

    [ObservableProperty] private FlickrDetails? _details;

    [ObservableProperty] private string _photoId = string.Empty;

    public PhotoDetailsViewModel(IFlickrApiService flickr)
    {
        _flickr = flickr;

        Debug.WriteLine("----------> PhotoDetailsViewModel constructor called");
    }

    public string CommentsHeaderTitle => $"Comments ({Comments?.Count ?? 0})";

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine("photoId changed: " + value + "");
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        Details = null;
        Comments.Clear();
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
            Debug.WriteLine("Getting comments for PhotoId: " + PhotoId);
            var comments = await _flickr.GetCommentsAsync(PhotoId);
            if (comments == null || comments.Count == 0)
            {
                Debug.WriteLine("No comments found.");
                OnPropertyChanged(nameof(CommentsHeaderTitle));
            }
            else
            {
                Debug.WriteLine($"Received {comments.Count} comments.");

                var tempComments = new ObservableCollection<FlickrComment>(comments);
                Comments = tempComments;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error getting comments: {e.Message}");
        }
    }
}