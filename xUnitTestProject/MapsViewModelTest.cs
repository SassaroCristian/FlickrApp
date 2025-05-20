using Xunit;
using Moq;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using FlickrApp.ViewModels.Base;
using Sensors = Microsoft.Maui.Devices.Sensors;

public class MapsViewModelTest
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IFlickrApiService> _mockFlickrApiService;
    private readonly MapsViewModel _viewModel;

    private FlickrPhoto CreateFakeFlickrPhoto(string id, string title = "Test Photo") =>
        new() { Id = id, Title = title };

    public MapsViewModelTest()
    {
        _mockNavigationService = new Mock<INavigationService>();
        _mockFlickrApiService = new Mock<IFlickrApiService>();

        _viewModel = new MapsViewModel(
            _mockNavigationService.Object,
            _mockFlickrApiService.Object
        );
    }

    // Verifies that the constructor initializes the Wonders collection and default property values.
    [Fact]
    public void Constructor_InitializesWondersAndDefaultProperties()
    {
        Assert.NotNull(_viewModel.Wonders);
        Assert.True(_viewModel.Wonders.Count >= 16);
        Assert.Contains(_viewModel.Wonders, w => w.Name == "Great Wall of China");

        Assert.Null(_viewModel.SelectedWonder);
        Assert.False(_viewModel.IsPinned);
        Assert.False(_viewModel.IsListFull);
        Assert.Equal(string.Empty, _viewModel.Location);
        Assert.NotNull(_viewModel.Photos);
        Assert.Empty(_viewModel.Photos);
    }

    // Tests if AddPinToMap sets pin status, (attempts to set location), and loads photos successfully.
    [Fact]
    public async Task AddPinToMap_SetsPinStatusAndLoadsPhotosSuccessfully()
    {
        double testLatitude = 45.0;
        double testLongitude = 12.0;

        var photosFromApi = new List<FlickrPhoto>
        {
            CreateFakeFlickrPhoto("1"),
            CreateFakeFlickrPhoto("2")
        };

        _mockFlickrApiService.Setup(f => f.GetForLocationAsync(
                testLatitude.ToString(CultureInfo.InvariantCulture),
                testLongitude.ToString(CultureInfo.InvariantCulture),
                1,
                4
            ))
            .ReturnsAsync(photosFromApi);

        await _viewModel.AddPinToMap(testLatitude, testLongitude);

        Assert.True(_viewModel.IsPinned);
        // Location property test is omitted due to static Geocoding call.

        _mockFlickrApiService.Verify(f => f.GetForLocationAsync(
            testLatitude.ToString(CultureInfo.InvariantCulture),
            testLongitude.ToString(CultureInfo.InvariantCulture), 1, 4), Times.Once);

        Assert.Equal(photosFromApi.Count, _viewModel.Photos.Count);
        Assert.Equal(photosFromApi[0].Id, _viewModel.Photos[0].Id);
        Assert.True(_viewModel.IsListFull);
        Assert.False(_viewModel.IsBusy);
    }

    // Checks behavior of AddPinToMap when the Flickr service returns no photos for a location.
    [Fact]
    public async Task AddPinToMap_WhenNoPhotosReturned_PhotosIsEmptyAndIsListFullIsFalse()
    {
        double testLatitude = 46.0;
        double testLongitude = 13.0;
        var emptyPhotosList = new List<FlickrPhoto>();

        _mockFlickrApiService.Setup(f => f.GetForLocationAsync(
                testLatitude.ToString(CultureInfo.InvariantCulture),
                testLongitude.ToString(CultureInfo.InvariantCulture),
                1, 4
            ))
            .ReturnsAsync(emptyPhotosList);

        await _viewModel.AddPinToMap(testLatitude, testLongitude);

        Assert.True(_viewModel.IsPinned);
        _mockFlickrApiService.Verify(f => f.GetForLocationAsync(
            testLatitude.ToString(CultureInfo.InvariantCulture),
            testLongitude.ToString(CultureInfo.InvariantCulture), 1, 4), Times.Once);

        Assert.Empty(_viewModel.Photos);
        Assert.False(_viewModel.IsListFull);
        Assert.False(_viewModel.IsBusy);
    }

    // Verifies that the GoToMapResults command navigates with the correct pin coordinates.
    [Fact]
    public async Task GoToMapResultsCommand_NavigatesWithCorrectPinCoordinatesAfterPinAdded()
    {
        double pinLat = 35.123;
        double pinLon = -70.456;

        _mockFlickrApiService.Setup(f =>
                f.GetForLocationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<FlickrPhoto>());
        await _viewModel.AddPinToMap(pinLat, pinLon);

        _mockNavigationService.Setup(n => n.GoToAsync(
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, object>>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _viewModel.GoToMapResultsCommand.ExecuteAsync(null);

        _mockNavigationService.Verify(n => n.GoToAsync(
            "MapResultsPage",
            It.Is<IDictionary<string, object>>(d =>
                d["Latitude"].ToString() == pinLat.ToString(CultureInfo.InvariantCulture) &&
                d["Longitude"].ToString() == pinLon.ToString(CultureInfo.InvariantCulture))
        ), Times.Once);
        Assert.False(_viewModel.IsBusy);
    }

    // Tests if calling AddPinToMap multiple times correctly reloads photos for the new location.
    [Fact]
    public async Task AddPinToMap_CalledMultipleTimes_ReloadsPhotosForNewLocation()
    {
        double lat1 = 40.0, lon1 = 10.0;
        double lat2 = 41.0, lon2 = 11.0;

        var photos1 = new List<FlickrPhoto> { CreateFakeFlickrPhoto("p1") };
        var photos2 = new List<FlickrPhoto> { CreateFakeFlickrPhoto("p2"), CreateFakeFlickrPhoto("p3") };

        _mockFlickrApiService.Setup(f => f.GetForLocationAsync(
                lat1.ToString(CultureInfo.InvariantCulture), lon1.ToString(CultureInfo.InvariantCulture), 1, 4))
            .ReturnsAsync(photos1);

        _mockFlickrApiService.Setup(f => f.GetForLocationAsync(
                lat2.ToString(CultureInfo.InvariantCulture), lon2.ToString(CultureInfo.InvariantCulture), 1, 4))
            .ReturnsAsync(photos2);

        await _viewModel.AddPinToMap(lat1, lon1);

        Assert.Equal(photos1.Count, _viewModel.Photos.Count);
        Assert.Contains(_viewModel.Photos, p => p.Id == "p1");
        Assert.True(_viewModel.IsPinned);
        Assert.True(_viewModel.IsListFull);

        await _viewModel.AddPinToMap(lat2, lon2);

        _mockFlickrApiService.Verify(f => f.GetForLocationAsync(
                lat1.ToString(CultureInfo.InvariantCulture), lon1.ToString(CultureInfo.InvariantCulture), 1, 4),
            Times.Once);
        _mockFlickrApiService.Verify(f => f.GetForLocationAsync(
                lat2.ToString(CultureInfo.InvariantCulture), lon2.ToString(CultureInfo.InvariantCulture), 1, 4),
            Times.Once);

        Assert.Equal(photos2.Count, _viewModel.Photos.Count);
        Assert.DoesNotContain(_viewModel.Photos, p => p.Id == "p1");
        Assert.Contains(_viewModel.Photos, p => p.Id == "p2");
        Assert.Contains(_viewModel.Photos, p => p.Id == "p3");
        Assert.True(_viewModel.IsPinned);
        Assert.True(_viewModel.IsListFull);
        Assert.False(_viewModel.IsBusy);
    }
}