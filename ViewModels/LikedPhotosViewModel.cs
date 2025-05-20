using FlickrApp.Entities;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels.Base;

namespace FlickrApp.ViewModels;

public class LikedPhotosViewModel : PhotoListViewModelBase
{
    private ILocalFileSystemService _fileSystem;
    private readonly IPhotoRepository _photoRepository;

    public LikedPhotosViewModel(
        IPhotoRepository photoRepository,
        IDeviceService device,
        ILocalFileSystemService fileSystem,
        INavigationService navigation) : base(navigation, device)
    {
        _photoRepository = photoRepository;
        _fileSystem = fileSystem;

        _ = InitializeAsync();
    }

    private async Task<List<PhotoEntity>> GetAllPhotos(int pageNumber, int pageSize)
    {
        return await ExecuteSafelyAsync(async () =>
        {
            var photos = await _photoRepository.GetAllPhotosAsync(pageNumber, pageSize);
            return photos;
        }) ?? [];
    }

    protected override async Task<ICollection<PhotoEntity>> FetchItemsAsync(int page, int perPage)
    {
        return await GetAllPhotos(page, perPage);
    }

    protected override async Task<ICollection<PhotoEntity>> FetchMoreItemsAsync(int page, int perPage)
    {
        return await GetAllPhotos(page, perPage);
    }
}