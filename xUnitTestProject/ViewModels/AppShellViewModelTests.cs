using FlickrApp.Models;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.ViewModels;

public class AppShellViewModelTests
{
    private readonly Mock<IFlickrApiService> _mockFlickr = new();

    private AppShellViewModel CreateSut()
    {
        return new AppShellViewModel(_mockFlickr.Object);
    }

    [Fact]
    public async Task Constructor_WhenCreatingCorrectly_GetBackgroundList()
    {
        var flickrMethodCalledTcs = new TaskCompletionSource<bool>();

        var backgrounds = new List<FlickrPhoto>
        {
            new() { Id = "1", MediumUrl = "http://example.com/photo1.jpg" },
            new() { Id = "2", MediumUrl = "http://example.com/photo2.jpg" }
        };

        _mockFlickr.Setup(f => f.SearchAsync(
                "background",
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                1,
                10,
                null
            ))
            .ReturnsAsync(backgrounds)
            .Callback(() => flickrMethodCalledTcs.SetResult(true));

        var sut = CreateSut();

        var timeout = TimeSpan.FromSeconds(2);
        var completedTask = await Task.WhenAny(flickrMethodCalledTcs.Task, Task.Delay(timeout));

        Assert.True(completedTask == flickrMethodCalledTcs.Task, "Timed out");

        _mockFlickr.Verify(f => f.SearchAsync(
            "background",
            string.Empty,
            null,
            null,
            null,
            null,
            null,
            1,
            10,
            null
        ), Times.Once);

        Assert.False(string.IsNullOrEmpty(sut.HeaderBackgroundSource));
        Assert.True(sut.HeaderBackgroundSource.Equals(backgrounds[0].MediumUrl) ||
                    sut.HeaderBackgroundSource.Equals(backgrounds[1].MediumUrl));
    }

    [Fact]
    public async Task OnFlyoutOpen_WhenIsTrue_DoNothing()
    {
        var flickrMethodCalledTcs = new TaskCompletionSource<bool>();

        var backgrounds = new List<FlickrPhoto>
        {
            new() { Id = "1", MediumUrl = "http://example.com/photo1.jpg" },
            new() { Id = "2", MediumUrl = "http://example.com/photo2.jpg" }
        };

        _mockFlickr.Setup(f => f.SearchAsync(
                "background",
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                1,
                10,
                null
            ))
            .ReturnsAsync(backgrounds)
            .Callback(() => flickrMethodCalledTcs.SetResult(true));

        var sut = CreateSut();

        var timeout = TimeSpan.FromSeconds(2);
        await Task.WhenAny(flickrMethodCalledTcs.Task, Task.Delay(timeout));

        var backgroundUrl = sut.HeaderBackgroundSource;
        sut.IsFlyoutOpen = true;

        Assert.Equal(backgroundUrl, sut.HeaderBackgroundSource);
    }

    [Fact]
    public async Task OnFlyoutOpen_WhenIsFalse_ChangeBackgroundSource()
    {
        var flickrMethodCalledTcs = new TaskCompletionSource<bool>();

        var backgrounds = new List<FlickrPhoto>
        {
            new() { Id = "1", MediumUrl = "http://example.com/photo1.jpg" },
            new() { Id = "2", MediumUrl = "http://example.com/photo2.jpg" }
        };

        _mockFlickr.Setup(f => f.SearchAsync(
                "background",
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                1,
                10,
                null
            ))
            .ReturnsAsync(backgrounds)
            .Callback(() => flickrMethodCalledTcs.SetResult(true));

        var sut = CreateSut();

        var timeout = TimeSpan.FromSeconds(2);
        await Task.WhenAny(flickrMethodCalledTcs.Task, Task.Delay(timeout));

        var backgroundUrl = sut.HeaderBackgroundSource;
        sut.IsFlyoutOpen = true;
        sut.IsFlyoutOpen = false;

        Assert.False(string.IsNullOrEmpty(sut.HeaderBackgroundSource));
        Assert.NotEqual(backgroundUrl, sut.HeaderBackgroundSource);
        Assert.True(sut.HeaderBackgroundSource.Equals(backgrounds[0].MediumUrl) ||
                    sut.HeaderBackgroundSource.Equals(backgrounds[1].MediumUrl));
    }
}