using System.Collections.ObjectModel;
using AutoMapper;
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
    ILocalFileSystemService fileService,
    IMapper mapper) : BaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CommentsHeaderTitle))]
    private ObservableCollection<FlickrComment> _comments = [];

    [ObservableProperty] private DetailEntity? _detail;
    [ObservableProperty] private string _photoId = string.Empty;
    [ObservableProperty] private bool _isFavorite;
    [ObservableProperty] private bool _isDownloaded;

    public string CommentsHeaderTitle => $"Comments ({Comments?.Count})";

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine($" ---> ID changed to: {value}");
        _ = FillData();
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        
            if (Detail == null)
            {
                Debug.WriteLine("ToggleFavoriteCommand: Detail are null. Cannot toggle favorite status.");
                return;
            }

            await ExecuteSafelyAsync(async () =>
            {
                if (IsFavorite)
                {
                    Debug.WriteLine($"Removing photo {PhotoId} from favorites (DB and potentially file).");
                    var photoToRemove = await photoRepository.GetPhotoByIdAsync(PhotoId);

                    if (!string.IsNullOrEmpty(photoToRemove?.LocalFilePath))
                    {
                        await fileService.DeleteFileAsync(photoToRemove.LocalFilePath);
                        Debug.WriteLine($"File System: Deleted local file: {photoToRemove.LocalFilePath}");
                    }

                    var deletedRows = await photoRepository.DeletePhotoAsync(PhotoId);
                    Debug.WriteLine($"DB: Deleted {deletedRows} row(s) for photo ID {PhotoId}.");

                    IsFavorite = false;
                }
                else
                {
                    Debug.WriteLine($"Adding photo {PhotoId} to favorites (DB and local file).");

                    var photoToSave = Detail.Photo;
                    if (photoToSave == null) return;
                    
                    var targetDirectory = fileService.GetAppSpecificPhotosDirectory();
                    var localFilePath =
                        await fileService.SaveImageAsync(Detail.Photo!.LargeUrl, Detail.Id, targetDirectory);

                    photoToSave.LocalFilePath = localFilePath;

                    var rowsAffected = await photoRepository.AddPhotoAsync(photoToSave);
                    Debug.WriteLine($"DB: Saved/Updated {rowsAffected} row(s) for photo ID {PhotoId}.");

                    IsFavorite = true;
                }
            });
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        /*
            if (IsDownloaded)
            {
                Debug.WriteLine("DownloadCommand: Already downloaded. Skipping.");
                return;
            }

            if (Detail == null)
            {
                Debug.WriteLine("DownloadCommand: Detail are null. Cannot download.");
                return;
            }

            await ExecuteSafelyAsync(async () =>
            {
                Debug.WriteLine($"Attempting to download image for photo ID: {PhotoId} from URL: {Detail.LargeImageUrl}");

                var targetDirectory = fileService.GetAppSpecificDownloadsDirectory();
                var localFilePath = await fileService.SaveImageAsync(Detail.LargeImageUrl, Detail.Id, targetDirectory);

                if (localFilePath != null)
                {
                    Debug.WriteLine($"Download successful. File saved at: {localFilePath}");
                    IsDownloaded = true;
                }
                else
                {
                    Debug.WriteLine($"Download failed for photo ID: {PhotoId}");
                }
            });*/
    }


    private async Task FillData()
    {
        Detail = null;
        Comments.Clear();

        // GETTING DETAILS
        await ExecuteSafelyAsync(async () =>
        {
            Debug.WriteLine(" ---> Getting details ...");

            DetailEntity? detail = null;
            var isPhotoSavedLocally = await photoRepository.IsPhotoSavedLocallyAsync(PhotoId);
            if (isPhotoSavedLocally)
            {
                var photo = await photoRepository.GetPhotoWithDetailByIdAsync(PhotoId);
                Debug.WriteLine(photoRepository.StatusMessage);
                detail = photo?.Detail;
                IsFavorite = true;
            }
            else
            {
                var item = await flickr.GetDetailsAsync(PhotoId);
                Debug.WriteLine(" --> Getting details from flickr service");
                detail = mapper.Map<DetailEntity>(item);
            }
            Detail = detail;
        });

        // GETTING COMMENTS
        _ = ExecuteSafelyAsync(async () =>
        {
            Debug.WriteLine(" --- Getting comments ...");
            var comments = await flickr.GetCommentsAsync(PhotoId);
            Comments = new ObservableCollection<FlickrComment>(comments);
        });
    }
}