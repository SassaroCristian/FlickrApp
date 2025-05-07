using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FlickrApp.Models;
using FlickrApp.Services;
using CommunityToolkit.Mvvm.Input;
using Debug = System.Diagnostics.Debug;
using System.Linq;
using System.Threading.Tasks;
using FlickrApp.Repositories;
using FlickrApp.Entities;


namespace FlickrApp.ViewModels;

[QueryProperty(nameof(PhotoId), nameof(PhotoId))]
public partial class PhotoDetailsViewModel : ObservableObject
{
    private readonly IFlickrApiService _flickr;
    private readonly ILocalFileSystemService _fileService;
    private readonly IPhotoRepository _photoRepository;


    [ObservableProperty] private bool _isFavorite;
    [ObservableProperty] private bool _isDownloaded = false;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CommentsHeaderTitle))]
    private ObservableCollection<FlickrComment> _comments = [];

    [ObservableProperty] private FlickrDetails? _details;

    [ObservableProperty] private string _photoId = string.Empty;


    public PhotoDetailsViewModel(IFlickrApiService flickr, IPhotoRepository photoRepository,
        ILocalFileSystemService fileService)
    {
        _flickr = flickr;
        _photoRepository = photoRepository;
        _fileService = fileService;

        Debug.WriteLine("----------> PhotoDetailsViewModel constructor called (ALL DEPENDENCIES)");
    }


    public string CommentsHeaderTitle => $"Comments ({Comments?.Count ?? 0})";

    partial void OnPhotoIdChanged(string value)
    {
        Debug.WriteLine("photoId changed: " + value + "");
        Task.Run(FillData);
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Details == null)
        {
            Debug.WriteLine("ToggleFavoriteCommand: Details are null. Cannot toggle favorite status.");
            return;
        }

        try
        {
            if (IsFavorite)
            {
                Debug.WriteLine($"Removing photo {PhotoId} from favorites (DB and potentially file).");

                var photoToRemove = await _photoRepository.GetPhotoByIdAsync(PhotoId);

                var deletedRows = await _photoRepository.DeletePhotoAsync(PhotoId);
                Debug.WriteLine($"DB: Deleted {deletedRows} row(s) for photo ID {PhotoId}.");

                if (photoToRemove != null && !string.IsNullOrEmpty(photoToRemove.LocalFilePath))
                {
                    await _fileService.DeleteFileAsync(photoToRemove.LocalFilePath);
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
                    Server = Details.Server,
                    Farm = Details.Farm,
                    DateUploaded = Details.Dates?.Posted,
                    Views = Details.Views,
                    LocalFilePath = null
                };

                var targetDirectory = _fileService.GetAppSpecificPhotosDirectory();
                var localFilePath =
                    await _fileService.SaveImageAsync(Details.LargeImageUrl, Details.Id, targetDirectory);

                photoToSave.LocalFilePath = localFilePath;

                var rowsAffected = await _photoRepository.SavePhotoAsync(photoToSave);
                Debug.WriteLine($"DB: Saved/Updated {rowsAffected} row(s) for photo ID {PhotoId}.");

                IsFavorite = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"### ERROR during ToggleFavoriteAsync for photo {PhotoId}: {ex.Message}");
        }
        finally
        {
        }
    }

    [RelayCommand]
    private async Task DownloadAsync()
    {
        if (IsDownloaded)
        {
            Debug.WriteLine("DownloadCommand: Already downloaded. Skipping.");
            return;
        }

        if (Details == null)
        {
            Debug.WriteLine("DownloadCommand: Details are null. Cannot download.");
            return;
        }

        Debug.WriteLine($"Attempting to download image for photo ID: {PhotoId} from URL: {Details.LargeImageUrl}");

        var targetDirectory = _fileService.GetAppSpecificDownloadsDirectory();
        var localFilePath = await _fileService.SaveImageAsync(Details.LargeImageUrl, Details.Id, targetDirectory);

        if (localFilePath != null)
        {
            Debug.WriteLine($"Download successful. File saved at: {localFilePath}");
            IsDownloaded = true;
        }
        else
        {
            Debug.WriteLine($"Download failed for photo ID: {PhotoId}");
        }
    }


    private async Task FillData()
    {
        Details = null;
        Comments.Clear();
        IsFavorite = false;
        IsDownloaded = false;

        // --- Load Details ---
        try
        {
            Debug.WriteLine($"Getting details for PhotoId: {PhotoId}");
            var details = await _flickr.GetDetailsAsync(PhotoId);
            Details = details;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"### ERROR getting details for {PhotoId}: {e.Message}");
        }

        // --- Load Comments ---
        try
        {
            Debug.WriteLine($"Getting comments for PhotoId: " + PhotoId);
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
            Debug.WriteLine($"### ERROR getting comments for {PhotoId}: {e.Message}");
        }

        // Check initial state from DB
        try
        {
            bool isSaved = await _photoRepository.IsPhotoSavedLocallyAsync(PhotoId);

            IsFavorite = isSaved;
            IsDownloaded = isSaved;

            Debug.WriteLine($"Photo {PhotoId} initial favorite/download status (checked via DB): {isSaved}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"### ERROR checking initial favorite/download status for {PhotoId}: {ex.Message}");
            IsFavorite = false;
            IsDownloaded = false;
        }
    }
}