using AutoMapper;
using FlickrApp.Models;
using FlickrApp.Models.Lookups;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class SearchViewModelTests
{
    private readonly SearchViewModel _sut;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IDeviceService> _mockDeviceService;
    private readonly Mock<IFlickrApiService> _mockFlickrApiService;
    private readonly Mock<IMapper> _mockMapper;

    public SearchViewModelTests()
    {
        _mockNavigationService = new Mock<INavigationService>();
        _mockDeviceService = new Mock<IDeviceService>();
        _mockFlickrApiService = new Mock<IFlickrApiService>();
        _mockMapper = new Mock<IMapper>();

        _sut = new SearchViewModel(_mockNavigationService.Object, _mockDeviceService.Object,
            _mockFlickrApiService.Object, _mockMapper.Object);
    }

    [Fact]
    public void SearchCommand_WhenExecuteWithValidTermsAndDeviceIsPhone_RedirectWithParams()
    {
        const string expectedRoute = "SearchResultPage";
        const string expectedSearchText = "dawn";
        var expectedSortCriterion = new SortCriterion("DateTakenDesc", "Date Taken Desc");
        const string expectedSearchTags = "nature,wonder";
        var expectedStartDate = new DateTime(2024, 1, 1);
        var expectedEndDate = new DateTime(2024, 12, 31);
        var expectedLicense = new FlickrLicense { Id = 1, Name = "Creative Commons" };
        var expectedContentType = new PickerItem(1, "Example Content Type");
        var expectedGeoContext = new PickerItem(1, "Example Geo Context");

        _sut.SearchText = expectedSearchText;
        _sut.SelectedSortCriterion = expectedSortCriterion;
        _sut.SearchTags = expectedSearchTags;
        _sut.StartDate = expectedStartDate;
        _sut.EndDate = expectedEndDate;
        _sut.SelectedLicense = expectedLicense;
        _sut.SelectedContentType = expectedContentType;
        _sut.SelectedGeoContext = expectedGeoContext;

        SearchNavigationParams capturedParams = null;

        _mockDeviceService.Setup(x => x.Idiom).Returns(DeviceIdiom.Phone);

        _mockNavigationService.Setup(nav => nav.GoToAsync(
                It.Is<string>(route => route == expectedRoute),
                It.Is<IDictionary<string, object>>(p =>
                    p.ContainsKey("SearchParameters") && p["SearchParameters"] is SearchNavigationParams)))
            .Callback<string, IDictionary<string, object>>((route, dict) =>
            {
                if (dict["SearchParameters"] is SearchNavigationParams searchParams) capturedParams = searchParams;
            });

        _sut.SearchCommand.Execute(null);

        _mockNavigationService.Verify(nav =>
            nav.GoToAsync(expectedRoute,
                It.Is<IDictionary<string, object>>(p =>
                    p.ContainsKey("SearchParameters") &&
                    p["SearchParameters"] is SearchNavigationParams &&
                    ((SearchNavigationParams)p["SearchParameters"]).SearchText != null
                    && ((SearchNavigationParams)p["SearchParameters"]).SearchText!.Equals(expectedSearchText)
                    && ((SearchNavigationParams)p["SearchParameters"]).SearchTags!.Equals(expectedSearchTags)
                    && ((SearchNavigationParams)p["SearchParameters"]).StartDate == expectedStartDate
                    && ((SearchNavigationParams)p["SearchParameters"]).EndDate == expectedEndDate
                    && ((SearchNavigationParams)p["SearchParameters"]).LicenseId == expectedLicense.Id.ToString()
                    && ((SearchNavigationParams)p["SearchParameters"]).ContentType ==
                    expectedContentType.Value.ToString()
                    && ((SearchNavigationParams)p["SearchParameters"]).SortCriterionValue == expectedSortCriterion.Value
                )
            ), Times.Once());
    }

    [Fact]
    public void SearchCommand_WhenExecuteWithValidTermsAndDeviceIsTablet_PopulatePhotos()
    {
        const string expectedRoute = "SearchResultPage";
        const string expectedSearchText = "dawn";
        var expectedSortCriterion = new SortCriterion("DateTakenDesc", "Date Taken Desc");
        const string expectedSearchTags = "nature,wonder";
        var expectedStartDate = new DateTime(2024, 1, 1);
        var expectedEndDate = new DateTime(2024, 12, 31);
        var expectedLicense = new FlickrLicense { Id = 1, Name = "Creative Commons" };
        var expectedContentType = new PickerItem(1, "Example Content Type");
        var expectedGeoContext = new PickerItem(1, "Example Geo Context");

        _sut.SearchText = expectedSearchText;
        _sut.SelectedSortCriterion = expectedSortCriterion;
        _sut.SearchTags = expectedSearchTags;
        _sut.StartDate = expectedStartDate;
        _sut.EndDate = expectedEndDate;
        _sut.SelectedLicense = expectedLicense;
        _sut.SelectedContentType = expectedContentType;
        _sut.SelectedGeoContext = expectedGeoContext;

        _mockDeviceService.Setup(x => x.Idiom).Returns(DeviceIdiom.Tablet);

        var photosToReturn = new List<FlickrPhoto>
        {
            new() { Id = "10" },
            new() { Id = "11" }
        };

        _mockFlickrApiService
            .Setup(service => service.SearchAsync(
                expectedSearchText,
                expectedSearchTags,
                expectedStartDate,
                expectedEndDate,
                expectedLicense.Id.ToString(),
                expectedContentType.Value.ToString(),
                expectedGeoContext.Value.ToString(),
                1,
                21,
                expectedSortCriterion.Value
            ))
            .ReturnsAsync(photosToReturn);

        _sut.SearchCommand.Execute(null);

        _mockFlickrApiService.Verify(flickr => flickr.SearchAsync(
            expectedSearchText,
            expectedSearchTags,
            expectedStartDate,
            expectedEndDate,
            expectedLicense.Id.ToString(),
            expectedContentType.Value.ToString(),
            expectedGeoContext.Value.ToString(),
            1,
            21,
            expectedSortCriterion.Value
        ), Times.Once());
    }

    [Fact]
    public void SearchCommand_WhenExecuteWithNoParametersAndDeviceIsTablet_DoNothing()
    {
        _mockDeviceService.Setup(dev => dev.Idiom).Returns(DeviceIdiom.Tablet);

        _sut.SearchCommand.Execute(null);

        _mockFlickrApiService.Verify(api => api.SearchAsync(It.IsAny<string>(), // per searchText
            It.IsAny<string>(),
            It.IsAny<DateTime?>(),
            It.IsAny<DateTime?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>()), Times.Never);
    }
}