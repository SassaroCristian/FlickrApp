using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;
using FlickrApp.Models.Lookups;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class SearchResultViewModelTests
{
    private readonly Mock<INavigationService> _mockNavigation = new();
    private readonly Mock<IDeviceService> _mockDevice = new();
    private readonly Mock<IFlickrApiService> _mockFlickr = new();
    private readonly Mock<IMapper> _mockMapper = new();

    private SearchResultViewModel CreateSut()
    {
        return new SearchResultViewModel(
            _mockNavigation.Object,
            _mockDevice.Object,
            _mockFlickr.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task OnSearchParametersChanged_WhenParamsValid_FillCollection()
    {
        var sut = CreateSut();
        var searchParams = new SearchNavigationParams
        {
            SearchText = "cats",
            SearchTags = "cute,fluffy",
            SortCriterionValue = "date-posted-desc"
        };
        var flickrPhotosDto = new List<FlickrPhoto>
        {
            new() { Id = "1", Title = "Cat 1" },
            new() { Id = "2", Title = "Cat 2" }
        };
        var mappedPhotoEntities = new List<PhotoEntity>
        {
            new() { Id = "1", Title = "Cat 1" },
            new() { Id = "2", Title = "Cat 2" }
        };

        var searchAsyncCalledTcs = new TaskCompletionSource<bool>();

        _mockFlickr.Setup(f => f.SearchAsync(
                searchParams.SearchText,
                searchParams.SearchTags,
                searchParams.StartDate,
                searchParams.EndDate,
                searchParams.LicenseId,
                searchParams.ContentType,
                searchParams.GeoContext,
                It.IsAny<int>(), // page
                It.IsAny<int>(), // perPage
                searchParams.SortCriterionValue))
            .ReturnsAsync(flickrPhotosDto)
            .Callback(() => searchAsyncCalledTcs.TrySetResult(true));

        for (var i = 0; i < flickrPhotosDto.Count; i++)
        {
            var dto = flickrPhotosDto[i];
            var entity = mappedPhotoEntities[i];
            _mockMapper.Setup(m => m.Map<PhotoEntity>(dto)).Returns(entity);
        }

        sut.SearchParameters = searchParams;

        var completedTask = await Task.WhenAny(searchAsyncCalledTcs.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(completedTask == searchAsyncCalledTcs.Task, "SearchAsync was not called within timeout.");

        _mockFlickr.Verify(f => f.SearchAsync(
                searchParams.SearchText,
                searchParams.SearchTags,
                searchParams.StartDate,
                searchParams.EndDate,
                searchParams.LicenseId,
                searchParams.ContentType,
                searchParams.GeoContext,
                It.IsAny<int>(),
                It.IsAny<int>(),
                searchParams.SortCriterionValue),
            Times.Once);
    }

    [Fact]
    public async Task LoadMoreItemsCommand_WhenMoreItemsAvailable_CallsSearchMoreAsync()
    {
        var searchParams = new SearchNavigationParams { SearchText = "dogs" };

        _mockDevice.Setup(d => d.Idiom).Returns(DeviceIdiom.Phone);
        const int initialPage = 1;
        const int nextPageAfterLoadMore = 2;

        var initialLoadDtos = Enumerable.Range(0, It.IsAny<int>())
            .Select(i => new FlickrPhoto { Id = $"initial_{i}", Title = $"Initial Dog {i}" }).ToList();
        var initialLoadEntities = initialLoadDtos
            .Select(dto => new PhotoEntity { Id = dto.Id, Title = dto.Title }).ToList();

        var initialLoadTcs = new TaskCompletionSource<bool>();

        _mockFlickr.Setup(f => f.SearchAsync(
                searchParams.SearchText, searchParams.SearchTags, searchParams.StartDate, searchParams.EndDate,
                searchParams.LicenseId, searchParams.ContentType, searchParams.GeoContext,
                initialPage, It.IsAny<int>(), searchParams.SortCriterionValue))
            .ReturnsAsync(initialLoadDtos)
            .Callback(() => initialLoadTcs.TrySetResult(true));

        for (var i = 0; i < initialLoadDtos.Count; i++)
            _mockMapper.Setup(m => m.Map<PhotoEntity>(initialLoadDtos[i])).Returns(initialLoadEntities[i]);

        var sut = CreateSut();

        sut.SearchParameters = searchParams;
        var initialLoadCompletedTask = await Task.WhenAny(initialLoadTcs.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(initialLoadCompletedTask == initialLoadTcs.Task,
            "Initial SearchAsync (from InitializeAsync) did not complete within timeout.");

        var loadMoreDtos = Enumerable.Range(0, It.IsAny<int>())
            .Select(i => new FlickrPhoto { Id = $"more_{i}", Title = $"More Dog {i}" }).ToList();
        var loadMoreEntities = loadMoreDtos
            .Select(dto => new PhotoEntity { Id = dto.Id, Title = dto.Title }).ToList();

        var searchMoreAsyncTcs = new TaskCompletionSource<bool>();

        _mockFlickr.Setup(f => f.SearchMoreAsync(
                searchParams.SearchText, searchParams.SearchTags, searchParams.StartDate, searchParams.EndDate,
                searchParams.LicenseId, searchParams.ContentType, searchParams.GeoContext,
                nextPageAfterLoadMore, It.IsAny<int>(), searchParams.SortCriterionValue))
            .ReturnsAsync(loadMoreDtos)
            .Callback(() => searchMoreAsyncTcs.TrySetResult(true));

        for (var i = 0; i < loadMoreDtos.Count; i++)
            _mockMapper.Setup(m => m.Map<PhotoEntity>(loadMoreDtos[i])).Returns(loadMoreEntities[i]);

        sut.AreMoreItemsAvailable = true;
        await sut.LoadMoreItemsCommand.ExecuteAsync(null);

        var searchMoreCompletedTask = await Task.WhenAny(searchMoreAsyncTcs.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(searchMoreCompletedTask == searchMoreAsyncTcs.Task,
            "SearchMoreAsync was not called within timeout after LoadMoreItemsCommand execution.");

        _mockFlickr.Verify(f => f.SearchMoreAsync(
                searchParams.SearchText, searchParams.SearchTags, searchParams.StartDate, searchParams.EndDate,
                searchParams.LicenseId, searchParams.ContentType, searchParams.GeoContext,
                nextPageAfterLoadMore, It.IsAny<int>(), searchParams.SortCriterionValue),
            Times.Once);
    }

    [Fact]
    public async Task LoadMoreItemsCommand_WhenNoMoreItemsAvailable_DoesNotCallSearchMoreAsync()
    {
        var sut = CreateSut();
        var searchParams = new SearchNavigationParams { SearchText = "birds" };

        _mockDevice.Setup(d => d.Idiom).Returns(DeviceIdiom.Phone);
        const int initialPage = 1;
        const int itemsLessThanPerPage = 5;

        var initialLoadDtos = Enumerable.Range(0, itemsLessThanPerPage)
            .Select(i => new FlickrPhoto { Id = $"initial_bird_{i}", Title = $"Initial Bird {i}" }).ToList();
        var initialLoadEntities = initialLoadDtos
            .Select(dto => new PhotoEntity { Id = dto.Id, Title = dto.Title }).ToList();

        var initialLoadTcs = new TaskCompletionSource<bool>();

        _mockFlickr.Setup(f => f.SearchAsync(
                searchParams.SearchText, searchParams.SearchTags, searchParams.StartDate, searchParams.EndDate,
                searchParams.LicenseId, searchParams.ContentType, searchParams.GeoContext,
                initialPage, It.IsAny<int>(), searchParams.SortCriterionValue))
            .ReturnsAsync(initialLoadDtos)
            .Callback(() => initialLoadTcs.TrySetResult(true));

        for (var i = 0; i < initialLoadDtos.Count; i++)
            _mockMapper.Setup(m => m.Map<PhotoEntity>(initialLoadDtos[i])).Returns(initialLoadEntities[i]);

        sut.SearchParameters = searchParams;
        var initialLoadCompletedTask = await Task.WhenAny(initialLoadTcs.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        Assert.True(initialLoadCompletedTask == initialLoadTcs.Task,
            "Initial SearchAsync (for NoMoreItems test) did not complete within timeout.");

        sut.AreMoreItemsAvailable = false;
        await sut.LoadMoreItemsCommand.ExecuteAsync(null);

        _mockFlickr.Verify(f => f.SearchMoreAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()),
            Times.Never);
    }
}