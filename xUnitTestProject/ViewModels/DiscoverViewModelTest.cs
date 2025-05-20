using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class DiscoverViewModelTest
{
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<IFlickrApiService> _mockFlickrApiService;
    private readonly Mock<IDeviceService> _mockDeviceService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DiscoverViewModel _viewModel;

    private PhotoEntity CreateFakePhotoEntity(string id, string title = "Test Photo") =>
        new() { Id = id, Title = title };

    private FlickrPhoto CreateFakeFlickrPhoto(string photoId, string title = "Flickr Photo") =>
        new() { Id = photoId, Title = title };

    public DiscoverViewModelTest()
    {
        _mockDeviceService = new Mock<IDeviceService>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockFlickrApiService = new Mock<IFlickrApiService>();
        _mockMapper = new Mock<IMapper>();

        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null,
                It.IsAny<string>(),
                null,
                null,
                null,
                null,
                null,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(new List<FlickrPhoto>());

        _mockFlickrApiService.Setup(f => f.SearchMoreAsync(
                null,
                It.IsAny<string>(),
                null,
                null,
                null,
                null,
                null,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            ))
            .ReturnsAsync(new List<FlickrPhoto>());

        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.IsAny<FlickrPhoto>()))
            .Returns((FlickrPhoto source) =>
                new PhotoEntity { Id = source?.Id ?? string.Empty, Title = source?.Title });

        _viewModel = new DiscoverViewModel(
            _mockNavigationService.Object,
            _mockDeviceService.Object,
            _mockFlickrApiService.Object,
            _mockMapper.Object);
    }

    // --- Test del Costruttore e Inizializzazione ---

    // Verifies that the available sort options are initialized correctly by the constructor.
    [Fact]
    public void Constructor_InitializesAvailableSortOptions()
    {
        Assert.NotNull(_viewModel.AvailableSortOptions);
        Assert.Equal(4, _viewModel.AvailableSortOptions.Count);
        Assert.Contains(_viewModel.AvailableSortOptions, o => o.SortEnumValue == FlickrSortOption.InterestingnessDesc);
        Assert.Contains(_viewModel.AvailableSortOptions, o => o.SortEnumValue == FlickrSortOption.InterestingnessAsc);
        Assert.Contains(_viewModel.AvailableSortOptions, o => o.SortEnumValue == FlickrSortOption.DatePostedDesc);
        Assert.Contains(_viewModel.AvailableSortOptions, o => o.SortEnumValue == FlickrSortOption.DatePostedAsc);
    }

    // Checks if the constructor sets the default sort option and its display name as expected.
    [Fact]
    public void Constructor_SetsDefaultSortOptionAndDisplayName()
    {
        Assert.Equal(FlickrSortOption.InterestingnessDesc, _viewModel.SelectedSortOption);
        Assert.Equal("Most Interesting", _viewModel.SelectedSortOptionDisplayName);
        Assert.NotNull(_viewModel.SelectedSortOptionItem);
        Assert.Equal(FlickrSortOption.InterestingnessDesc, _viewModel.SelectedSortOptionItem.SortEnumValue);
    }

    // Ensures the constructor triggers an initial data fetch via InitializeAsync and FetchItemsAsync.
    [Fact]
    public async Task Constructor_CallsInitializeAsync_WhichCallsFetchItemsAsyncViaBase()
    {
        var localFlickrMock = new Mock<IFlickrApiService>();
        var localMapperMock = new Mock<IMapper>();
        var photosFromApi = new List<FlickrPhoto> { CreateFakeFlickrPhoto("1", "Test API Photo") };

        localFlickrMock.Setup(f => f.SearchAsync(
                null,
                It.Is<string>(tags => tags != null && tags.Contains("-naked,-Naked")),
                null,
                null,
                null,
                null,
                null,
                1,
                It.IsAny<int>(),
                "interestingness-desc"
            ))
            .ReturnsAsync(photosFromApi)
            .Verifiable();

        localMapperMock.Setup(m => m.Map<PhotoEntity>(It.IsAny<FlickrPhoto>()))
            .Returns((FlickrPhoto p) => new PhotoEntity { Id = p.Id, Title = p.Title });

        var localViewModel = new DiscoverViewModel(
            _mockNavigationService.Object,
            _mockDeviceService.Object,
            localFlickrMock.Object,
            localMapperMock.Object);

        await Task.Delay(200);

        localFlickrMock.Verify();
    }

    // --- Test per la Logica di Ordinamento ---

    // Tests if changing the selected sort option enum correctly updates dependent display properties.
    [Theory]
    [InlineData(FlickrSortOption.DatePostedDesc, "Newest First")]
    [InlineData(FlickrSortOption.InterestingnessAsc, "Least Interesting")]
    public void OnSelectedSortOptionChanged_UpdatesDisplayNameAndItem(FlickrSortOption newSortOption,
        string expectedDisplayName)
    {
        _viewModel.SelectedSortOption = newSortOption;

        Assert.Equal(expectedDisplayName, _viewModel.SelectedSortOptionDisplayName);
        Assert.NotNull(_viewModel.SelectedSortOptionItem);
        Assert.Equal(newSortOption, _viewModel.SelectedSortOptionItem.SortEnumValue);
    }

    // Checks if changing the selected sort option item updates the sort option enum and refreshes data.
    [Fact]
    public async Task OnSelectedSortOptionItemChanged_UpdatesSortOptionAndRefreshesData()
    {
        var newSortOptionItem =
            _viewModel.AvailableSortOptions.First(o => o.SortEnumValue == FlickrSortOption.DatePostedDesc);
        _mockFlickrApiService.Invocations.Clear();

        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null, It.IsAny<string>(), null, null, null, null, null,
                1,
                It.IsAny<int>(),
                "date-posted-desc"
            ))
            .ReturnsAsync(new List<FlickrPhoto> { CreateFakeFlickrPhoto("sorted") })
            .Verifiable();

        _viewModel.SelectedSortOptionItem = newSortOptionItem;
        await Task.Delay(100);

        Assert.Equal(FlickrSortOption.DatePostedDesc, _viewModel.SelectedSortOption);
        _mockFlickrApiService.Verify();
    }

    // Verifies that executing the sort order command updates the option and refreshes data.
    [Fact]
    public async Task SetSortOrderCommand_UpdatesSortOptionAndRefreshesData()
    {
        var newSortOption = FlickrSortOption.DatePostedAsc;
        _mockFlickrApiService.Invocations.Clear();
        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null, It.IsAny<string>(), null, null, null, null, null,
                1, It.IsAny<int>(), "date-posted-asc"
            ))
            .ReturnsAsync(new List<FlickrPhoto> { CreateFakeFlickrPhoto("new_sort") })
            .Verifiable();

        await _viewModel.SetSortOrderCommand.ExecuteAsync(newSortOption);
        await Task.WhenAny(Task.Run(async () =>
        {
            while (_viewModel.IsBusy) await Task.Delay(10);
        }), Task.Delay(500));

        Assert.Equal(newSortOption, _viewModel.SelectedSortOption);
        _mockFlickrApiService.Verify();
    }

    // --- Test per GetSortOrderApiString (indiretto) ---

    // Tests if the GetSortOrderApiString method's output is correctly used when setting a sort order.
    [Theory]
    [InlineData(FlickrSortOption.InterestingnessDesc, "interestingness-desc")]
    [InlineData(FlickrSortOption.DatePostedAsc, "date-posted-asc")]
    public async Task GetSortOrderApiString_IsUsedCorrectly_WhenSettingSortOrder(FlickrSortOption option,
        string expectedApiSortString)
    {
        _mockFlickrApiService.Invocations.Clear();
        // This setup now expects the specific sort string.
        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null,
                It.IsAny<string>(),
                null,
                null,
                null,
                null,
                null,
                It.IsAny<int>(),
                It.IsAny<int>(),
                expectedApiSortString // Verify correct sort string is passed
            ))
            .ReturnsAsync(new List<FlickrPhoto>())
            .Verifiable(); // Added Verifiable to ensure this specific setup is hit.

        await _viewModel.SetSortOrderCommand.ExecuteAsync(option);
        await Task.WhenAny(Task.Run(async () =>
        {
            while (_viewModel.IsBusy) await Task.Delay(10);
        }), Task.Delay(500));

        _mockFlickrApiService.Verify();
    }

    // --- Test per il Filtro per Tag ---

    // Checks if the filter by tag command correctly updates filter display, resets sort, and refreshes data.
    [Theory]
    [InlineData("landscape", "Landscape")]
    [InlineData(null, "Popular")]
    public async Task FilterByTagCommand_UpdatesFilterAndResetsSortAndRefreshes(string? tag,
        string expectedDisplayTitle)
    {
        var expectedApiTags = string.IsNullOrWhiteSpace(tag) ? "-naked,-Naked" : $"{tag},-naked,-Naked";
        _mockFlickrApiService.Invocations.Clear();
        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null,
                expectedApiTags,
                null, null, null, null, null,
                1,
                It.IsAny<int>(),
                "interestingness-desc"
            ))
            .ReturnsAsync(new List<FlickrPhoto> { CreateFakeFlickrPhoto("filtered") })
            .Verifiable();

        await _viewModel.FilterByTagCommand.ExecuteAsync(tag);
        await Task.WhenAny(Task.Run(async () =>
        {
            while (_viewModel.IsBusy) await Task.Delay(10);
        }), Task.Delay(500));

        Assert.Equal(expectedDisplayTitle, _viewModel.FilterDisplayTitle);
        Assert.Equal(FlickrSortOption.InterestingnessDesc, _viewModel.SelectedSortOption);
        _mockFlickrApiService.Verify();
    }

    // --- Test per FetchItemsAsync e FetchMoreItemsAsync ---

    // Verifies that FetchItemsAsync is called with correct parameters when triggered by a sort command.
    [Fact]
    public async Task FetchItemsAsync_IsCalledWithCorrectParameters_WhenTriggered()
    {
        await _viewModel.FilterByTagCommand.ExecuteAsync("mountains");
        var specificSortOption = FlickrSortOption.DatePostedAsc;
        var expectedApiSortString = "date-posted-asc";
        var expectedApiTags = "mountains,-naked,-Naked";

        _mockFlickrApiService.Invocations.Clear();
        var photosFromApi = new List<FlickrPhoto> { CreateFakeFlickrPhoto("p1"), CreateFakeFlickrPhoto("p2") };
        var mappedPhotos = photosFromApi.Select(p => CreateFakePhotoEntity(p.Id, p.Title)).ToList();

        _mockFlickrApiService.Setup(f => f.SearchAsync(
                null,
                expectedApiTags,
                null, null, null, null, null,
                1,
                It.IsAny<int>(),
                expectedApiSortString
            ))
            .ReturnsAsync(photosFromApi)
            .Verifiable();

        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(p => p.Id == "p1"))).Returns(mappedPhotos[0]);
        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(p => p.Id == "p2"))).Returns(mappedPhotos[1]);

        await _viewModel.SetSortOrderCommand.ExecuteAsync(specificSortOption);

        await Task.WhenAny(Task.Run(async () =>
        {
            while (_viewModel.IsBusy) await Task.Delay(20);
        }), Task.Delay(1000));

        _mockFlickrApiService.Verify();
        _mockMapper.Verify(m => m.Map<PhotoEntity>(It.IsAny<FlickrPhoto>()), Times.Exactly(photosFromApi.Count));

        Assert.Equal(photosFromApi.Count,
            _viewModel.Photos
                .Count);
        Assert.Contains(_viewModel.Photos, p => p.Id == "p1");
        Assert.Contains(_viewModel.Photos, p => p.Id == "p2");
    }

    // Tests if the "load more items" command calls the Flickr service with correct pagination and parameters.
    [Fact]
    public async Task LoadMoreItemsCommand_CallsFlickrServiceSearchMoreWithCorrectParameters()
    {
        _viewModel.AreMoreItemsAvailable = true;
        var nextPageToFetch = 2;

        string currentTagForApi = string.Empty;
        if (_viewModel.FilterDisplayTitle != "Popular")
        {
            currentTagForApi = _viewModel.FilterDisplayTitle.ToLower();
        }

        var expectedApiTags = string.IsNullOrWhiteSpace(currentTagForApi)
            ? "-naked,-Naked"
            : $"{currentTagForApi},-naked,-Naked";

        var currentSortOption = _viewModel.SelectedSortOption;
        string expectedApiSortString = currentSortOption switch
        {
            FlickrSortOption.InterestingnessDesc => "interestingness-desc",
            FlickrSortOption.InterestingnessAsc => "interestingness-asc",
            FlickrSortOption.DatePostedDesc => "date-posted-desc",
            FlickrSortOption.DatePostedAsc => "date-posted-asc",
            _ => "interestingness-desc"
        };

        _mockFlickrApiService.Invocations.Clear();
        var morePhotosFromApi = new List<FlickrPhoto> { CreateFakeFlickrPhoto("p_more") };
        _mockFlickrApiService.Setup(f => f.SearchMoreAsync(
                null,
                expectedApiTags,
                null, null, null, null, null,
                nextPageToFetch,
                It.IsAny<int>(),
                expectedApiSortString
            ))
            .ReturnsAsync(morePhotosFromApi)
            .Verifiable();

        _mockMapper.Setup(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(p => p.Id == "p_more")))
            .Returns(CreateFakePhotoEntity("p_more"));

        await _viewModel.LoadMoreItemsCommand.ExecuteAsync(null);
        await Task.WhenAny(Task.Run(async () =>
        {
            while (_viewModel.IsBusy) await Task.Delay(10);
        }), Task.Delay(500));

        _mockFlickrApiService.Verify();
        _mockMapper.Verify(m => m.Map<PhotoEntity>(It.Is<FlickrPhoto>(p => p.Id == "p_more")), Times.AtLeastOnce());
        Assert.Contains(_viewModel.Photos, p => p.Id == "p_more");
    }
}