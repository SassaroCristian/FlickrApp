using FlickrApp.Models;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public class LikedPhotosViewModel : PhotoListViewModelBase
{
    private ILocalFileSystemService _fileSystem;
    private readonly IPhotoRepository _photoRepository;
    private const int perPageInit = 10;

    public LikedPhotosViewModel(
        IPhotoRepository photoRepository,
        ILocalFileSystemService fileSystem,
        INavigationService navigation) : base(navigation)
    {
        _photoRepository = photoRepository;
        _fileSystem = fileSystem;

        _ = InitializeAsync(perPageInit);
    }

    private async Task<List<FlickrPhoto>> GetAllPhotos(int pageNumber, int pageSize)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var items = await _photoRepository.GetAllPhotosAsync(pageNumber, pageSize);

            var photos = new List<FlickrPhoto>();
            foreach (var photoEntity in items)
            {
                if (!await _photoRepository.IsPhotoSavedLocallyAsync(photoEntity.Id)) continue;
                photos.Add(new FlickrPhoto
                {
                    Id = photoEntity.Id,
                    Owner = photoEntity.OwnerNsid ?? string.Empty,
                    Secret = photoEntity.Secret,
                    Server = photoEntity.Server,
                    Farm = photoEntity.Farm,
                    Title = photoEntity.Title ?? string.Empty,
                    MediumUrl = photoEntity.LocalFilePath
                });
            }

            return photos;
        }) ?? [];
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchItemsAsync(int page, int perPage)
    {
        return await GetAllPhotos(page, perPage);
    }

    protected override async Task<ICollection<FlickrPhoto>> FetchMoreItemsAsync(int page, int perPage)
    {
        return await GetAllPhotos(page, perPage);
    }
}