using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using FlickrApp.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace FlickrApp.Services;

public class FlickrApiService(HttpClient httpClient) : IFlickrApiService
{
    private const string baseUrl = "https://www.flickr.com/services/rest/";
    private const string apiKey = "255ac8fdac4726aa339fa1c2161b9e5b";

    private DateTime _getRecentMaxUploadDate = DateTime.UtcNow;
    private DateTime _searchMaxUploadDate = DateTime.UtcNow;
    private DateTime _getForLocationMaxUploadDate = DateTime.UtcNow;

    public async Task<List<FlickrPhoto>> GetRecentAsync(int page = 1, int perPage = 10)
    {
        _getRecentMaxUploadDate = DateTime.UtcNow;
        return await GetMoreRecentAsync(page, perPage);
    }

    public async Task<List<FlickrPhoto>> GetMoreRecentAsync(int page = 1, int perPage = 10)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "method", "flickr.photos.search" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "page", page.ToString() },
            { "per_page", perPage.ToString() },
            { "sort", "date-posted-desc" },
            { "max_upload_date", _getRecentMaxUploadDate.ToString(CultureInfo.InvariantCulture) }
        };

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        Debug.WriteLine(requestUrl);

        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.GetRecent>(json);

        return finalResponse != null ? [..finalResponse.Photos.List] : [];
    }

    public async Task<List<FlickrPhoto>> SearchAsync(string text, string tags, int page = 1, int perPage = 10)
    {
        _searchMaxUploadDate = DateTime.UtcNow;
        return await SearchMoreAsync(text, tags, page, perPage);
    }

    public async Task<List<FlickrPhoto>> SearchMoreAsync(string text, string tags, int page = 1, int perPage = 10)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "method", "flickr.photos.search" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "page", page.ToString() },
            { "per_page", perPage.ToString() },
            { "max_upload_date", _searchMaxUploadDate.ToString(CultureInfo.InvariantCulture) }
        };

        if (!string.IsNullOrEmpty(text)) queryParams.Add("text", text);
        if (!string.IsNullOrEmpty(tags)) queryParams.Add("tags", tags);

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);

        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.Search>(json);

        return finalResponse != null ? [..finalResponse.Photos.List] : [];
    }

    public Task<List<FlickrPhoto>> GetForLocationAsync(double latitude, double longitude, int accuracy = 1,
        int page = 1,
        int perPage = 10)
    {
        throw new NotImplementedException();
    }

    public Task<List<FlickrPhoto>> GetMoreForLocationAsync(double latitude, double longitude, int accuracy = 1,
        int page = 1, int perPage = 10)
    {
        throw new NotImplementedException();
    }

    public async Task<FlickrDetails?> GetDetailsAsync(string photoId)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "method", "flickr.photos.getInfo" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "photo_id", photoId }
        };

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);

        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.GetDetails>(json);

        return finalResponse?.Details;
    }

    public async Task<List<FlickrComment>> GetCommentsAsync(string photoId)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "method", "flickr.photos.comments.getList" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "photo_id", photoId }
        };

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        Debug.WriteLine(requestUrl);

        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.GetComments>(json);

        return finalResponse.Comments.List != null ? [..finalResponse.Comments.List] : [];
    }
}