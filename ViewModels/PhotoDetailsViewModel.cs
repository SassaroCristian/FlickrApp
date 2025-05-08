using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;
using Debug = System.Diagnostics.Debug;


namespace FlickrApp.ViewModels;

[QueryProperty(nameof(PhotoId), nameof(PhotoId))]
public partial class PhotoDetailsViewModel(
    IFlickrApiService flickr,
    IPhotoRepository photoRepository,
    ILocalFileSystemService fileService) : BaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CommentsHeaderTitle))]
    private ObservableCollection<FlickrComment> _comments = [];

    [ObservableProperty] private FlickrDetails? _details;
    [ObservableProperty] private string _photoId = string.Empty;
    [ObservableProperty] private bool _isFavorite;
    

    public string CommentsHeaderTitle => $"Comments ({Comments?.Count})";

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine($"photoId changed: {value}");
        _ = FillData();
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Details == null)
        {
            Debug.WriteLine("ToggleFavoriteCommand: Details are null. Cannot toggle favorite status.");
            return;
        }

        await ExecuteSafelyAsync(async () =>
        {
            if (IsFavorite)
            {
                Debug.WriteLine($"Removing photo {PhotoId} from favorites (DB and potentially file).");

                var photoToRemove = await photoRepository.GetPhotoByIdAsync(PhotoId);

                var deletedRows = await photoRepository.DeletePhotoAsync(PhotoId);
                Debug.WriteLine($"DB: Deleted {deletedRows} row(s) for photo ID {PhotoId}.");

                if (!string.IsNullOrEmpty(photoToRemove.LocalFilePath))
                {
                    await fileService.DeleteFileAsync(photoToRemove.LocalFilePath);
                    Debug.WriteLine($"File System: Deleted local file: {photoToRemove.LocalFilePath}");
                }

                IsFavorite = false;
            }
            else
            {
                Debug.WriteLine($"Adding photo {PhotoId} to favorites (DB and local file).");

                var photoToSave = new Photo
                {
                    Id = Details.Id,
                    Title = Details.Title?.Content,
                    Description = Details.Description?.Content,
                    OwnerNsid = Details.Owner?.Nsid,
                    OwnerUsername = Details.Owner?.Username,
                    Secret = Details.Secret,
                    Farm = Details.Farm,
                    DateUploaded = Details.Dates?.Posted,
                    Views = Details.Views,
                    LocalFilePath = null
                };

                var targetDirectory = fileService.GetAppSpecificPhotosDirectory();
                var localFilePath =
                    await fileService.SaveImageAsync(Details.LargeImageUrl, Details.Id, targetDirectory);

                photoToSave.LocalFilePath = localFilePath;

                var rowsAffected = await photoRepository.SavePhotoAsync(photoToSave);
                Debug.WriteLine($"DB: Saved/Updated {rowsAffected} row(s) for photo ID {PhotoId}.");

                IsFavorite = true;
            }
        });
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        if (Details == null)
        {
            Debug.WriteLine("DownloadCommand: Details are null. Cannot download.");
            return;
        }

        await ExecuteSafelyAsync(async () =>
        {
            Debug.WriteLine($"Attempting to download image for photo ID: {PhotoId} from URL: {Details.LargeImageUrl}");

            var targetDirectory = fileService.GetAppSpecificDownloadsDirectory();
            var localFilePath = await fileService.SaveImageAsync(Details.LargeImageUrl, Details.Id, targetDirectory);

            Debug.WriteLine(localFilePath != null
                ? $"Download successful. File saved at: {localFilePath}"
                : $"Download failed for photo ID: {PhotoId}");

            Toast.Make(localFilePath != null
                ? $"Download successful\nFile saved at: {localFilePath}"
                : $"Download failed for photo ID: {PhotoId}");
        });
    }


    private async Task FillData()
    {
        Details = null;
        Comments.Clear();
        
        // GETTING DETAILS
        await ExecuteSafelyAsync(async () =>
        {
            Debug.WriteLine(" ---> Getting details ...");
            var isPhotoSavedLocally = await photoRepository.IsPhotoSavedLocallyAsync(PhotoId);

            if (isPhotoSavedLocally)
            {
                var photoEntity = await photoRepository.GetPhotoByIdAsync(PhotoId);
                Details = new FlickrDetails
                {
                    Id = photoEntity.Id,
                    Secret = photoEntity.Secret,
                    Server = photoEntity.Server,
                    Farm = photoEntity.Farm,
                    DateUploaded = photoEntity.DateUploaded,
                    Views = photoEntity.Views,
                    Title = new Title { Content = photoEntity.Title },
                    Description = new Description { Content = photoEntity.Description },
                    Owner = new Owner
                    {
                        Nsid = photoEntity.OwnerNsid,
                        Username = photoEntity.OwnerUsername
                    },
                    LargeImageUrl = photoEntity.LocalFilePath
                };
                IsFavorite = true;
            }
            else
            {
                Details = await flickr.GetDetailsAsync(PhotoId);
            }
        });

        // GETTING COMMENTS
        await ExecuteSafelyAsync(async () =>
        {
            Debug.WriteLine(" --- Getting comments ...");
            var comments = await flickr.GetCommentsAsync(PhotoId);
            Comments = new ObservableCollection<FlickrComment>(comments);
        });
    }
}