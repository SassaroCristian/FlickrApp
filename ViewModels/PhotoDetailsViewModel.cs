using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;
using Debug = System.Diagnostics.Debug;

namespace FlickrApp.ViewModels;

[QueryProperty(nameof(PhotoId), nameof(PhotoId))]
public partial class PhotoDetailsViewModel(IFlickrApiService flickr) : Base.BaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CommentsHeaderTitle))]
    private ObservableCollection<FlickrComment> _comments = [];

    [ObservableProperty] private FlickrDetails? _details;
    [ObservableProperty] private string _photoId = string.Empty;

    public string CommentsHeaderTitle => $"Comments ({Comments?.Count})";

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine("photoId changed: " + value + "");
        Task.Run(FillData);
    }

    private async Task FillData()
    {
        Details = null;
        Comments.Clear();

        // GETTING DETAILS
        await ExecuteSafelyAsync(async () =>
        {
            var details = await flickr.GetDetailsAsync(PhotoId);
            Details = details;
        });

        // GETTING COMMENTS
        await ExecuteSafelyAsync(async () =>
        {
            var comments = await flickr.GetCommentsAsync(PhotoId);
            Comments = new ObservableCollection<FlickrComment>(comments);
        });
    }
}