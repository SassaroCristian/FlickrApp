using FlickrApp.Locators;
using FlickrApp.Services;
using FlickrApp.ViewModels;
using Moq;

namespace xUnitTestProject.Locators;

public class ViewModelLocatorTests
{
    private static Mock<IServiceProvider> _mockServiceProvider { get; } = new();

    [Fact]
    public void Initialize_WhenServiceProviderIsNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => ViewModelLocator.Initialize(null));
    }

    [Fact]
    public void Resolve_WhenServiceProviderIsNull_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => ViewModelLocator.AppShellViewModel);
    }

    [Fact]
    public void Initialize_WhenServiceProviderIsValid_DoesNotThrowsException()
    {
        var exception = Record.Exception(() => ViewModelLocator.Initialize(_mockServiceProvider.Object));
        Assert.Null(exception);
    }

    [Fact]
    public void Resolve_WhenServiceProviderIsValid_ReturnViewModel()
    {
        var mockFlickr = new Mock<IFlickrApiService>();
        var vmToReturn = new AppShellViewModel(mockFlickr.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(AppShellViewModel))).Returns(vmToReturn);

        ViewModelLocator.Initialize(_mockServiceProvider.Object);

        var vm = ViewModelLocator.AppShellViewModel;

        _mockServiceProvider.Verify(sp => sp.GetService(typeof(AppShellViewModel)), Times.Once);
        Assert.NotNull(vm);
        Assert.IsType<AppShellViewModel>(vm);
    }
}