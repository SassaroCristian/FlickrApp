using System.Diagnostics;
using System.Net;
using FlickrApp.Services;
using Moq;
using Moq.Protected;

namespace xUnitTestProject.Services;

public class LocalFileSystemServiceTests : IDisposable
{
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<IFileSystemOperations> _mockFileSystem;
    private readonly LocalFileSystemService _sut;
    private readonly TestTraceListener _testListener;

    private class TestTraceListener : TraceListener
    {
        public List<string> Messages { get; } = new();

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            Messages.Add(message);
        }
    }

    public LocalFileSystemServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new Mock<HttpClient>(_mockHttpMessageHandler.Object);
        _mockFileSystem = new Mock<IFileSystemOperations>();
        _testListener = new TestTraceListener();
        Trace.Listeners.Add(_testListener);

        _mockFileSystem.Setup(fs => fs.GetAppDataDirectory()).Returns("/mock/appdata");

        _sut = new LocalFileSystemService(_mockHttpClient.Object, _mockFileSystem.Object);
    }

    public void Dispose()
    {
        Trace.Listeners.Remove(_testListener);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetAppSpecificPhotosDirectory_CreatesAndReturnsCorrectPath()
    {
        var expectedBase = "/mock/appdata";
        var expectedDirName = "DownloadedPhotos";
        var expectedPath = Path.Combine(expectedBase, expectedDirName);
        _mockFileSystem.Setup(fs => fs.Combine(expectedBase, expectedDirName)).Returns(expectedPath);

        var result = _sut.GetAppSpecificPhotosDirectory();

        Assert.Equal(expectedPath, result);
        _mockFileSystem.Verify(fs => fs.CreateDirectory(expectedPath), Times.Once);
    }

    [Fact]
    public void GetAppSpecificDownloadsDirectory_CreatesAndReturnsCorrectPath()
    {
        var expectedBase = "/mock/appdata";
        var expectedDirName = "GeneralDownloads";
        var expectedPath = Path.Combine(expectedBase, expectedDirName);
        _mockFileSystem.Setup(fs => fs.Combine(expectedBase, expectedDirName)).Returns(expectedPath);

        var result = _sut.GetAppSpecificDownloadsDirectory();

        Assert.Equal(expectedPath, result);
        _mockFileSystem.Verify(fs => fs.CreateDirectory(expectedPath), Times.Once);
    }

    [Fact]
    public async Task DeleteFileAsync_WhenPathIsNullOrEmpty_DoesNotCallDeleteAndLogs()
    {
        await _sut.DeleteFileAsync(null);
        await _sut.DeleteFileAsync(string.Empty);

        _mockFileSystem.Verify(fs => fs.DeleteFileAsync(It.IsAny<string>()), Times.Never);
        Assert.Contains(_testListener.Messages, msg => msg.Contains("File path is null/empty"));
    }

    [Fact]
    public async Task DeleteFileAsync_WhenFileDoesNotExist_DoesNotCallDeleteAndLogs()
    {
        var testPath = "/mock/appdata/somefile.txt";
        _mockFileSystem.Setup(fs => fs.FileExists(testPath)).Returns(false);

        await _sut.DeleteFileAsync(testPath);

        _mockFileSystem.Verify(fs => fs.DeleteFileAsync(testPath), Times.Never);
        Assert.Contains(_testListener.Messages, msg => msg.Contains("file does not exist"));
    }

    [Fact]
    public async Task DeleteFileAsync_WhenFileExists_CallsDeleteAndLogs()
    {
        var testPath = "/mock/appdata/existingfile.jpg";
        _mockFileSystem.Setup(fs => fs.FileExists(testPath)).Returns(true);
        _mockFileSystem.Setup(fs => fs.DeleteFileAsync(testPath)).Returns(Task.CompletedTask);

        await _sut.DeleteFileAsync(testPath);

        _mockFileSystem.Verify(fs => fs.DeleteFileAsync(testPath), Times.Once);
        Assert.Contains(_testListener.Messages, msg => msg.Contains($"File deleted: {testPath}"));
    }

    [Fact]
    public async Task DeleteFileAsync_WhenDeleteThrowsException_LogsError()
    {
        var testPath = "/mock/appdata/problemfile.dat";
        var exceptionMessage = "Access denied";
        _mockFileSystem.Setup(fs => fs.FileExists(testPath)).Returns(true);
        _mockFileSystem.Setup(fs => fs.DeleteFileAsync(testPath))
            .ThrowsAsync(new UnauthorizedAccessException(exceptionMessage));

        await _sut.DeleteFileAsync(testPath);

        _mockFileSystem.Verify(fs => fs.DeleteFileAsync(testPath), Times.Once);
        Assert.Contains(_testListener.Messages,
            msg => msg.Contains($"### ERROR deleting file {testPath}: {exceptionMessage}"));
    }

    [Theory]
    [InlineData(null, "id", "/target")]
    [InlineData("url", null, "/target")]
    [InlineData("url", "id", null)]
    public async Task SaveImageAsync_WithNullOrEmptyParameters_ReturnsNullAndLogs(string imageUrl, string photoId,
        string targetDir)
    {
        var result = await _sut.SaveImageAsync(imageUrl, photoId, targetDir);

        Assert.Null(result);
        _mockFileSystem.Verify(fs => fs.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        Assert.Contains(_testListener.Messages,
            msg => msg.Contains("SaveImageAsync: imageUrl, photoId, or targetDirectory is null/empty."));
    }

    [Fact]
    public async Task SaveImageAsync_WhenFileAlreadyExists_ReturnsPathAndSkipsDownload()
    {
        var imageUrl = "http://example.com/image.jpg";
        var photoId = "photo123";
        var targetDir = "/mock/downloads";
        var expectedFileName = $"{photoId}.jpg";
        var expectedFilePath = Path.Combine(targetDir, expectedFileName);

        _mockFileSystem.Setup(fs => fs.Combine(targetDir, expectedFileName)).Returns(expectedFilePath);
        _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(true);

        var result = await _sut.SaveImageAsync(imageUrl, photoId, targetDir);

        Assert.Equal(expectedFilePath, result);
        _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        _mockFileSystem.Verify(fs => fs.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        Assert.Contains(_testListener.Messages,
            msg => msg.Contains($"File already exists at: {expectedFilePath}. Skipping download."));
    }

    [Fact]
    public async Task SaveImageAsync_SuccessfulDownloadAndSave_ReturnsPathAndLogs()
    {
        var imageUrl = "http://example.com/newimage.png";
        var photoId = "newPhoto456";
        var targetDir = "/mock/appdata/DownloadedPhotos";
        var expectedFileName = $"{photoId}.png";
        var expectedFilePath = Path.Combine(targetDir, expectedFileName);
        var imageBytes = new byte[] { 1, 2, 3, 4 };

        _mockFileSystem.Setup(fs => fs.Combine(targetDir, expectedFileName)).Returns(expectedFilePath);
        _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get && req.RequestUri.ToString() == imageUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(imageBytes)
            });

        _mockFileSystem.Setup(fs => fs.WriteAllBytesAsync(expectedFilePath, imageBytes)).Returns(Task.CompletedTask);

        var result = await _sut.SaveImageAsync(imageUrl, photoId, targetDir);

        Assert.Equal(expectedFilePath, result);
        _mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>(
            "SendAsync", Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == imageUrl),
            ItExpr.IsAny<CancellationToken>());
        _mockFileSystem.Verify(fs => fs.WriteAllBytesAsync(expectedFilePath, imageBytes), Times.Once);
        Assert.Contains(_testListener.Messages, msg => msg.Contains($"Image saved locally to: {expectedFilePath}"));
    }

    [Fact]
    public async Task SaveImageAsync_WhenHttpCallFails_ReturnsNullAndLogsError()
    {
        var imageUrl = "http://example.com/brokenimage.jpg";
        var photoId = "brokenPhoto789";
        var targetDir = "/mock/downloads";
        var expectedFileName = $"{photoId}.jpg";
        var expectedFilePath = Path.Combine(targetDir, expectedFileName);

        _mockFileSystem.Setup(fs => fs.Combine(targetDir, expectedFileName)).Returns(expectedFilePath);
        _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await _sut.SaveImageAsync(imageUrl, photoId, targetDir);

        Assert.Null(result);
        _mockFileSystem.Verify(fs => fs.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        Assert.Contains(_testListener.Messages,
            msg => msg.Contains($"### ERROR saving image from URL {imageUrl}") && msg.Contains("Network error"));
    }

    [Fact]
    public async Task SaveImageAsync_WhenFileWriteFails_ReturnsNullAndLogsError()
    {
        var imageUrl = "http://example.com/goodimage.jpg";
        var photoId = "writeFailPhoto";
        var targetDir = "/mock/downloads";
        var expectedFileName = $"{photoId}.jpg";
        var expectedFilePath = Path.Combine(targetDir, expectedFileName);
        var imageBytes = new byte[] { 1, 2, 3 };

        _mockFileSystem.Setup(fs => fs.Combine(targetDir, expectedFileName)).Returns(expectedFilePath);
        _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new ByteArrayContent(imageBytes) });

        _mockFileSystem.Setup(fs => fs.WriteAllBytesAsync(expectedFilePath, imageBytes))
            .ThrowsAsync(new IOException("Disk full"));

        var result = await _sut.SaveImageAsync(imageUrl, photoId, targetDir);

        Assert.Null(result);
        Assert.Contains(_testListener.Messages,
            msg => msg.Contains($"### ERROR saving image from URL {imageUrl}") && msg.Contains("Disk full"));
    }
}