using System.Diagnostics;
using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Repositories;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class PhotoDetailsViewModelTests
{
    private readonly Mock<IFlickrApiService> _mockFlickrService = new();
    private readonly Mock<IPhotoRepository> _mockPhotoRepository = new();
    private readonly Mock<ILocalFileSystemService> _mockFileService = new();
    private readonly Mock<IMapper> _mockMapper = new();

    private PhotoDetailsViewModel CreateSut()
    {
        return new PhotoDetailsViewModel(
            _mockFlickrService.Object,
            _mockPhotoRepository.Object,
            _mockFileService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task OnPhotoIdChanged_WhenValidIdAndPhotoNotLocal_FillsDetailsFromFlickr()
    {
        var sut = CreateSut();
        const string testPhotoId = "test123";
        var flickrApiDetailDto = new FlickrDetails
        {
            Id = testPhotoId, Description = new Description { Content = "Flickr Description" },
            Title = new Title { Content = "Flickr Title" }
        };
        var expectedDetailEntity = new DetailEntity
        {
            Id = testPhotoId, Description = "Flickr Description",
            Photo = new PhotoEntity { Id = testPhotoId, Title = "Flickr Title" }
        };

        var fillDataCompletionSource = new TaskCompletionSource<bool>();

        _mockPhotoRepository.Setup(repo => repo.IsPhotoSavedLocallyAsync(testPhotoId))
            .ReturnsAsync(false);

        _mockFlickrService.Setup(flickr => flickr.GetDetailsAsync(testPhotoId))
            .ReturnsAsync(flickrApiDetailDto);

        _mockMapper.Setup(mapper => mapper.Map<DetailEntity>(flickrApiDetailDto))
            .Returns(expectedDetailEntity)
            .Callback(() => fillDataCompletionSource.TrySetResult(true));

        _mockFlickrService.Setup(flickr => flickr.GetCommentsAsync(It.IsAny<string>()))
            .ReturnsAsync([]);

        sut.PhotoId = testPhotoId;

        var completedTask = await Task.WhenAny(fillDataCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(completedTask == fillDataCompletionSource.Task,
            "FillData (specifically mapper.Map) did not complete within timeout.");


        _mockPhotoRepository.Verify(repo => repo.IsPhotoSavedLocallyAsync(testPhotoId), Times.Once);
        _mockFlickrService.Verify(flickr => flickr.GetDetailsAsync(testPhotoId), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<DetailEntity>(flickrApiDetailDto), Times.Once);
        Assert.NotNull(sut.Detail);
        Assert.Same(expectedDetailEntity, sut.Detail);
    }

    [Fact]
    public async Task OnPhotoIdChanged_WhenValidIdAndPhotoIsLocal_FillsDetailsFromRepository()
    {
        var sut = CreateSut();
        const string testPhotoId = "testLocal456";

        var expectedDetailFromRepo = new DetailEntity
            { Id = testPhotoId, Description = "Local Description" };

        var photoEntityFromRepo = new PhotoEntity
        {
            Id = testPhotoId,
            Title = "Local Photo Title",
            Detail = expectedDetailFromRepo
        };
        expectedDetailFromRepo.Photo = photoEntityFromRepo;

        var fillDataCompletionSource = new TaskCompletionSource<bool>();

        _mockPhotoRepository.Setup(repo => repo.IsPhotoSavedLocallyAsync(testPhotoId))
            .ReturnsAsync(true);

        _mockPhotoRepository.Setup(repo => repo.GetPhotoWithDetailByIdAsync(testPhotoId))
            .ReturnsAsync(photoEntityFromRepo)
            .Callback(() =>
            {
                Debug.WriteLine("TEST_LOCAL: GetPhotoWithDetailByIdAsync called.");
                fillDataCompletionSource.TrySetResult(true);
            });

        _mockFlickrService.Setup(flickr => flickr.GetCommentsAsync(It.IsAny<string>()))
            .ReturnsAsync([]);

        sut.PhotoId = testPhotoId;

        var completedTask = await Task.WhenAny(fillDataCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(completedTask == fillDataCompletionSource.Task,
            "FillData (specifically GetPhotoWithDetailByIdAsync) did not complete within timeout for Local test.");

        _mockPhotoRepository.Verify(repo => repo.IsPhotoSavedLocallyAsync(testPhotoId), Times.Once);
        _mockPhotoRepository.Verify(repo => repo.GetPhotoWithDetailByIdAsync(testPhotoId), Times.Once);

        _mockFlickrService.Verify(flickr => flickr.GetDetailsAsync(It.IsAny<string>()), Times.Never);
        _mockMapper.Verify(mapper => mapper.Map<DetailEntity>(It.IsAny<FlickrDetails>()), Times.Never);

        Assert.NotNull(sut.Detail);
        Assert.Same(expectedDetailFromRepo, sut.Detail);
        Assert.True(sut.IsFavorite, "IsFavorite should be true when loaded from local repository.");
    }
}