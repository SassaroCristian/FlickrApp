using FlickrApp.Entities;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class TestableLikedPhotosViewModel(
    IPhotoRepository photoRepository,
    IDeviceService device,
    ILocalFileSystemService fileSystem,
    INavigationService navigation)
    : LikedPhotosViewModel(photoRepository, device, fileSystem, navigation)
{
    public Task<ICollection<PhotoEntity>> CallFetchItemsAsync(int page, int perPage)
    {
        return base.FetchItemsAsync(page, perPage);
    }

    public Task<ICollection<PhotoEntity>> CallFetchMoreItemsAsync(int page, int perPage)
    {
        return base.FetchMoreItemsAsync(page, perPage);
    }

    public bool InitializeAsyncCalled { get; private set; }

    protected override Task InitializeAsync()
    {
        InitializeAsyncCalled = true;
        return base.InitializeAsync();
    }
}

public class LikedPhotosViewModelTests
{
    private readonly Mock<IPhotoRepository> _mockPhotoRepository = new();
    private readonly Mock<IDeviceService> _mockDeviceService = new();
    private readonly Mock<ILocalFileSystemService> _mockFileSystemService = new();
    private readonly Mock<INavigationService> _mockNavigationService = new();

    private TestableLikedPhotosViewModel CreateSut()
    {
        return new TestableLikedPhotosViewModel(
            _mockPhotoRepository.Object,
            _mockDeviceService.Object,
            _mockFileSystemService.Object,
            _mockNavigationService.Object
        );
    }

    [Fact]
    public void Constructor_CallsInitializeAsync()
    {
        var sut = CreateSut();
        Assert.True(sut.InitializeAsyncCalled);
    }
    
    [Fact]
    public async Task FetchItemsAsync_WhenRepositoryReturnsPhotos_ReturnsPopulatedList()
    {
        var sut = CreateSut();
        var page = 1;
        var perPage = 10;
        var photosFromRepo = new List<PhotoEntity>
        {
            new() { Id = "1", Title = "Test Photo 1" },
            new() { Id = "2", Title = "Test Photo 2" }
        };

        _mockPhotoRepository
            .Setup(repo => repo.GetAllPhotosAsync(page, perPage))
            .ReturnsAsync(photosFromRepo);

        var result = await sut.CallFetchItemsAsync(page, perPage);

        _mockPhotoRepository.Verify(repo => repo.GetAllPhotosAsync(page, perPage), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(photosFromRepo.Count, result.Count);
        Assert.True(result.SequenceEqual(photosFromRepo));
    }

    [Fact]
    public async Task FetchItemsAsync_WhenRepositoryReturnsEmptyList_ReturnsEmptyList()
    {
        var sut = CreateSut();
        var page = 1;
        var perPage = 10;
        var emptyPhotosFromRepo = new List<PhotoEntity>();

        _mockPhotoRepository
            .Setup(repo => repo.GetAllPhotosAsync(page, perPage))
            .ReturnsAsync(emptyPhotosFromRepo);

        var result = await sut.CallFetchItemsAsync(page, perPage);

        _mockPhotoRepository.Verify(repo => repo.GetAllPhotosAsync(page, perPage), Times.Once);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FetchItemsAsync_WhenRepositoryThrowsException_ReturnsEmptyList()
    {
        var sut = CreateSut();
        var page = 1;
        var perPage = 10;

        _mockPhotoRepository
            .Setup(repo => repo.GetAllPhotosAsync(page, perPage))
            .ThrowsAsync(new Exception("Simulated repo error"));

        var result = await sut.CallFetchItemsAsync(page, perPage);

        _mockPhotoRepository.Verify(repo => repo.GetAllPhotosAsync(page, perPage), Times.Once);
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FetchMoreItemsAsync_WhenRepositoryReturnsPhotos_ReturnsPopulatedList()
    {
        var sut = CreateSut();
        var page = 2;
        var perPage = 5;
        var photosFromRepo = new List<PhotoEntity>
        {
            new() { Id = "3", Title = "Test Photo 3" }
        };

        _mockPhotoRepository
            .Setup(repo => repo.GetAllPhotosAsync(page, perPage))
            .ReturnsAsync(photosFromRepo);

        var result = await sut.CallFetchMoreItemsAsync(page, perPage);

        _mockPhotoRepository.Verify(repo => repo.GetAllPhotosAsync(page, perPage), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(photosFromRepo.Count, result.Count);
        Assert.True(result.SequenceEqual(photosFromRepo));
    }
}