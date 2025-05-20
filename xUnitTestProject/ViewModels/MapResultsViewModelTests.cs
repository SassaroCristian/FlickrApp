using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class MapResultsViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigation = new();
    private readonly Mock<IDeviceService> _mockDevice = new();
    private readonly Mock<IFlickrApiService> _mockFlickr = new();
    private readonly Mock<IMapper> _mockMapper = new();

    private MapResultsViewModel CreateSut()
    {
        return new MapResultsViewModel(_mockNavigation.Object, _mockDevice.Object, _mockFlickr.Object,
            _mockMapper.Object);
    }

    private List<FlickrPhoto> GetPhotosDto()
    {
        return
        [
            new FlickrPhoto
            {
                Id = "53728745007",
                Owner = "12345678@N00",
                Secret = "abcdef1234",
                Server = "65535",
                Farm = 66,
                Title = "Beautiful Sunset",
                Ispublic = 1,
                Isfriend = 0,
                Isfamily = 0
            },

            new FlickrPhoto
            {
                Id = "53729998888",
                Owner = "87654321@N01",
                Secret = "fedcba4321",
                Server = "65530",
                Farm = 66,
                Title = "Mountain Landscape",
                Ispublic = 1,
                Isfriend = 0,
                Isfamily = 0
            },

            new FlickrPhoto
            {
                Id = "53720001111",
                Owner = "11223344@N02",
                Secret = "qwerty0987",
                Server = "65400",
                Farm = 65,
                Title = "City Lights",
                Ispublic = 1,
                Isfriend = 0,
                Isfamily = 0
            }
        ];
    }

    private List<PhotoEntity> GetPhotosEntity()
    {
        return
        [
            new PhotoEntity
            {
                Id = "53728745007",
                Title = "Beautiful Sunset",
                Secret = "abcdef1234",
                Server = "65535",
                LocalFilePath = null, // Or string.Empty
                Detail = null
            },

            new PhotoEntity
            {
                Id = "53729998888",
                Title = "Mountain Landscape",
                Secret = "fedcba4321",
                Server = "65530",
                LocalFilePath = null, // Or string.Empty
                Detail = null
            },

            new PhotoEntity
            {
                Id = "53720001111",
                Title = "City Lights",
                Secret = "qwerty0987",
                Server = "65400",
                LocalFilePath = null, // Or string.Empty
                Detail = null
            }
        ];
    }

    [Fact]
    public void OnLongitudeChanged_WhenLatitudeIsNotSet_TryLoadDataIsCalledAndShouldFail()
    {
        var sut = CreateSut();
        sut.Longitude = "13.2300";

        _mockFlickr.Verify(f => f.GetForLocationAsync(sut.Latitude, sut.Longitude, It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public void OnLatitudeChanged_WhenLongitudeIsNotSet_TryLoadDataIsCalledAndShouldFail()
    {
        var sut = CreateSut();
        sut.Latitude = "13.2300";

        _mockFlickr.Verify(f => f.GetForLocationAsync(sut.Latitude, sut.Longitude, It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task OnLongitudeChanged_WhenLatitudeIsSet_TryLoadDataIsCalledAndShouldSucceed()
    {
        var sut = CreateSut();

        var photoDtos = GetPhotosDto();
        var photoEntities = GetPhotosEntity();

        _mockFlickr.Setup(f => f.GetForLocationAsync(sut.Latitude, sut.Longitude, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(photoDtos);
        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(inputDto => inputDto.Id == photoDtos[0].Id)))
            .Returns(photoEntities[1]);
        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(inputDto => inputDto.Id == photoDtos[1].Id)))
            .Returns(photoEntities[0]);
        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(inputDto => inputDto.Id == photoDtos[2].Id)))
            .Returns(photoEntities[2]);

        await Task.Delay(TimeSpan.FromSeconds(3));

        sut.Latitude = "13.2300";
        sut.Longitude = "13.2300";

        _mockFlickr.Verify(f => f.GetForLocationAsync(sut.Latitude, sut.Longitude, It.IsAny<int>(), It.IsAny<int>()),
            Times.Once);

        Assert.NotNull(sut.Photos);
    }
}