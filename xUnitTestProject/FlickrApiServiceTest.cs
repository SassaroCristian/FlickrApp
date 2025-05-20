using Xunit;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using FlickrApp.Models;
using FlickrApp.Services;
using Microsoft.AspNetCore.WebUtilities;

public class FlickrApiServiceTest 
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly FlickrApiService _service;
    private const string BaseFlickrUrl = "https://www.flickr.com/services/rest/";
    private const string ApiKey = "255ac8fdac4726aa339fa1c2161b9e5b";

    public FlickrApiServiceTest()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new FlickrApiService(_httpClient);
    }

    private void SetupHttpHandlerResponse(HttpStatusCode statusCode, string jsonContent,
        HttpMethod? expectedMethod = null, string? expectedUrlContains = null)
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    (expectedMethod == null || req.Method == expectedMethod) &&
                    (expectedUrlContains == null || req.RequestUri.ToString().Contains(expectedUrlContains))
                ),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(jsonContent)
            })
            .Verifiable();
    }

    private string CreateFlickrPhotosResponseJson(List<FlickrPhoto> photos, string stat = "ok")
    {
        var apiResponse = new FlickrApiResponses.Search
        {
            Photos = new FlickrPhotos
            {
                Page = 1,
                Pages = 1,
                Perpage = photos.Count,
                Total = photos.Count,
                List = photos
            },
            Stat = stat
        };
        return JsonSerializer.Serialize(apiResponse);
    }

    private string CreateFlickrDetailsResponseJson(FlickrDetails? details, string stat = "ok")
    {
        var apiResponse = new FlickrApiResponses.GetDetails
        {
            Details = details,
            Stat = stat
        };
        return JsonSerializer.Serialize(apiResponse);
    }

    private string CreateFlickrLicensesResponseJson(List<FlickrLicense> licenses, string stat = "ok")
    {
        var apiResponse = new FlickrApiResponses.GetLicenses
        {
            Licenses = new Licenses { List = licenses },
            Stat = stat
        };
        return JsonSerializer.Serialize(apiResponse);
    }

    // Tests if GetMoreRecentAsync constructs the correct API URL and properly deserializes the photo list.
    [Fact]
    public async Task GetMoreRecentAsync_ConstructsCorrectUrlAndDeserializesPhotos()
    {
        var page = 2;
        var perPage = 5;
        var fakePhotos = new List<FlickrPhoto> { new() { Id = "1", Title = "Recent 1" } };
        var mockJsonResponse = CreateFlickrPhotosResponseJson(fakePhotos);
        // Using a less restrictive setup for SendAsync as Verify will check the specific request.
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new StringContent(mockJsonResponse) });

        var result = await _service.GetMoreRecentAsync(page, perPage);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(fakePhotos[0].Id, result[0].Id);

        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().StartsWith(BaseFlickrUrl) &&
                req.RequestUri.Query.Contains($"method=flickr.photos.search") &&
                req.RequestUri.Query.Contains($"api_key={ApiKey}") &&
                req.RequestUri.Query.Contains($"page={page}") &&
                req.RequestUri.Query.Contains($"per_page={perPage}") &&
                req.RequestUri.Query.Contains($"sort=date-posted-desc") &&
                req.RequestUri.Query.Contains($"max_upload_date=")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    // Tests if SearchMoreAsync constructs the correct API URL with search parameters (tags, sort order) and deserializes the photo list.
    [Fact]
    public async Task SearchMoreAsync_WithTagsAndSortOrder_ConstructsCorrectUrlAndDeserializesPhotos()
    {
        var tags = "sunset,beach";
        var sortOrder = "interestingness-desc";
        var page = 1;
        var perPage = 3;
        var fakePhotos = new List<FlickrPhoto> { new() { Id = "s1", Title = "Sunset Beach" } };
        var mockJsonResponse = CreateFlickrPhotosResponseJson(fakePhotos);

        HttpRequestMessage? actualRequest = null;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((request, token) => actualRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse)
            });

        // Re-instantiate service with the mock for THIS test if _service uses a shared _httpClient that might be configured by other tests.
        // Or ensure _service is new for each test (which it is with xUnit default behavior).
        var result = await _service.SearchMoreAsync(tags: tags, sortOrder: sortOrder, page: page, perPage: perPage);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(fakePhotos[0].Id, result[0].Id);

        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );

        Assert.NotNull(actualRequest);
        Assert.NotNull(actualRequest.RequestUri); // Added null check
        Assert.Equal(HttpMethod.Get, actualRequest.Method);
        Assert.StartsWith(BaseFlickrUrl, actualRequest.RequestUri.ToString());

        var queryParams = QueryHelpers.ParseQuery(actualRequest.RequestUri.Query);
        Assert.Equal("flickr.photos.search", queryParams["method"]);
        Assert.Equal(ApiKey, queryParams["api_key"]);
        Assert.Equal(tags, queryParams["tags"]);
        Assert.Equal("all", queryParams["tag_mode"]);
        Assert.Equal(sortOrder, queryParams["sort"]);
        Assert.Equal(page.ToString(), queryParams["page"]);
        Assert.Equal(perPage.ToString(), queryParams["per_page"]);
        Assert.True(queryParams.ContainsKey("max_upload_date"));
    }

    // Tests if GetMoreForLocationAsync constructs the correct API URL for geo-location search and deserializes the photo list.
    [Fact]
    public async Task GetMoreForLocationAsync_ConstructsCorrectUrlAndDeserializesPhotos()
    {
        var latitude = "45.123";
        var longitude = "-70.456";
        var page = 1;
        var perPage = 2;
        var fakePhotos = new List<FlickrPhoto> { new() { Id = "loc1", Title = "Location Pic" } };
        var mockJsonResponse = CreateFlickrPhotosResponseJson(fakePhotos);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new StringContent(mockJsonResponse) });

        var result = await _service.GetMoreForLocationAsync(latitude, longitude, page, perPage);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(fakePhotos[0].Id, result[0].Id);

        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().StartsWith(BaseFlickrUrl) &&
                req.RequestUri.Query.Contains($"method=flickr.photos.search") &&
                req.RequestUri.Query.Contains($"lat={latitude}") &&
                req.RequestUri.Query.Contains($"lon={longitude}") &&
                req.RequestUri.Query.Contains($"page={page}") &&
                req.RequestUri.Query.Contains($"per_page={perPage}") &&
                req.RequestUri.Query.Contains($"max_upload_date=")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    // Tests if GetDetailsAsync constructs the correct API URL for photo details and properly deserializes the response.
    [Fact]
    public async Task GetDetailsAsync_ConstructsCorrectUrlAndDeserializesDetails()
    {
        var photoId = "pid123";
        var fakeDetails = new FlickrDetails
            { Id = photoId, Title = new FlickrApp.Models.Title { Content = "Photo Details Test" } };
        var mockJsonResponse = CreateFlickrDetailsResponseJson(fakeDetails);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new StringContent(mockJsonResponse) });

        var result = await _service.GetDetailsAsync(photoId);

        Assert.NotNull(result);
        Assert.Equal(fakeDetails.Id, result.Id);
        Assert.NotNull(result.Title);
        Assert.Equal(fakeDetails.Title.Content, result.Title.Content);

        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().StartsWith(BaseFlickrUrl) &&
                req.RequestUri.Query.Contains($"method=flickr.photos.getInfo") &&
                req.RequestUri.Query.Contains($"photo_id={photoId}")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    // Tests if GetRecentAsync correctly sets the 'max_upload_date' and calls GetMoreRecentAsync, verifying the date in the final URL.
    [Fact]
    public async Task GetRecentAsync_SetsMaxUploadDate_AndCallsGetMoreRecentAsync()
    {
        var page = 1;
        var perPage = 10;
        var fakePhotos = new List<FlickrPhoto> { new() { Id = "gr1", Title = "GetRecent Call" } };
        var mockJsonResponse = CreateFlickrPhotosResponseJson(fakePhotos);

        HttpRequestMessage? actualRequest = null;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((request, token) => actualRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse)
            });

        var timeBeforeCall = DateTime.UtcNow;

        var result = await _service.GetRecentAsync(page, perPage);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(fakePhotos[0].Id, result[0].Id);

        Assert.NotNull(actualRequest);
        Assert.NotNull(actualRequest.RequestUri);

        var queryParams = QueryHelpers.ParseQuery(actualRequest.RequestUri.Query);
        Assert.True(queryParams.TryGetValue("max_upload_date", out var maxUploadDateString));
        Assert.NotNull(maxUploadDateString);
        Assert.True(DateTime.TryParse(maxUploadDateString, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var maxUploadDate));

        var timeBeforeCallRounded = new DateTime(timeBeforeCall.Year, timeBeforeCall.Month, timeBeforeCall.Day,
            timeBeforeCall.Hour, timeBeforeCall.Minute, timeBeforeCall.Second, DateTimeKind.Utc);

        Assert.True(maxUploadDate >= timeBeforeCallRounded && (maxUploadDate - timeBeforeCallRounded).TotalSeconds < 5,
            $"max_upload_date ({maxUploadDate:o}) non è corretto rispetto a timeBeforeCallRounded ({timeBeforeCallRounded:o}). L'URL era: {actualRequest.RequestUri}");
    }

    // Tests if an API call throws HttpRequestException when the HttpClient returns an error status code.
    [Fact]
    public async Task ApiCall_WhenHttpClientReturnsError_ThrowsHttpRequestException()
    {
        var mockJsonResponse = JsonSerializer.Serialize(new { stat = "fail", code = 1, message = "Not found" });
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.NotFound, Content = new StringContent(mockJsonResponse) });

        await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetDetailsAsync("anyPhotoId"));
    }

    // Tests if GetLicensesAsync constructs the correct API URL for licenses and properly deserializes the license list.
    [Fact]
    public async Task GetLicensesAsync_ConstructsCorrectUrlAndDeserializesLicenses()
    {
        var fakeLicenses = new List<FlickrLicense>
        {
            new() { Id = 1, Name = "All Rights Reserved", Url = "http://example.com/arr" },
            new() { Id = 4, Name = "Attribution License", Url = "http://example.com/by" }
        };
        var mockJsonResponse = CreateFlickrLicensesResponseJson(fakeLicenses);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new StringContent(mockJsonResponse) });

        var result = await _service.GetLicensesAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(fakeLicenses[0].Name, result[0].Name);
        Assert.Equal(fakeLicenses[1].Id, result[1].Id);

        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().StartsWith(BaseFlickrUrl) &&
                req.RequestUri.Query.Contains($"method=flickr.photos.licenses.getInfo") &&
                req.RequestUri.Query.Contains($"api_key={ApiKey}")
            ),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}