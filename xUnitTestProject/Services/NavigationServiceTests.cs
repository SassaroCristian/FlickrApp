using FlickrApp.Services;
using Moq;

namespace xUnitTestProject.Services;

public class NavigationServiceTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider = new ();
    private readonly Mock<IShellNavigation> _mockShellNavigation = new ();
    private readonly NavigationService _sut;

    public NavigationServiceTests()
    {
        _sut = new NavigationService(_mockServiceProvider.Object, _mockShellNavigation.Object);
    }

    [Fact]
    public async Task GoToAsync_WithoutParameters_CallsShellNavigationGoToAsyncCorrectly()
    {
        const string testRoute = "//HomePage";

        _mockShellNavigation.Setup(s => s.GoToAsync(
                It.Is<ShellNavigationState>(state => state.Location.OriginalString == testRoute)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.GoToAsync(testRoute);

        _mockShellNavigation.Verify();
    }

    [Fact]
    public async Task GoToAsync_WithParameters_CallsShellNavigationGoToAsyncWithParametersCorrectly()
    {
        const string testRoute = "//DetailPage";
        var testParameters = new Dictionary<string, object>
        {
            { "ItemId", 123 },
            { "ItemName", "TestItem" }
        };

        _mockShellNavigation.Setup(s => s.GoToAsync(
                It.Is<ShellNavigationState>(state => state.Location.OriginalString == testRoute),
                testParameters))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.GoToAsync(testRoute, testParameters);

        _mockShellNavigation.Verify();
    }

    [Fact]
    public async Task GoToAsync_WithEmptyParameters_CallsShellNavigationGoToAsyncWithoutParameters()
    {
        const string testRoute = "//ItemsPage";
        var emptyParameters = new Dictionary<string, object>();

        _mockShellNavigation.Setup(s => s.GoToAsync(
                It.Is<ShellNavigationState>(state => state.Location.OriginalString == testRoute)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.GoToAsync(testRoute, emptyParameters);

        _mockShellNavigation.Verify();
        // Verify that the overload with parameters was NOT called when parameters are empty
        _mockShellNavigation.Verify(s => s.GoToAsync(
            It.IsAny<ShellNavigationState>(),
            It.IsAny<IDictionary<string, object>>()), Times.Never);
    }

    [Fact]
    public async Task GoBackAsync_CallsShellNavigationGoToAsyncWithCorrectRoute()
    {
        const string expectedGoBackRoute = "..";

        _mockShellNavigation.Setup(s => s.GoToAsync(
                It.Is<ShellNavigationState>(state => state.Location.OriginalString == expectedGoBackRoute)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _sut.GoBackAsync();
        _mockShellNavigation.Verify();
    }
}