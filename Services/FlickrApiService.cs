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
    private DateTime _getForLocationMaxUploadDate = DateTime.UtcNow;
    private DateTime _getRecentMaxUploadDate = DateTime.UtcNow;
    private DateTime _searchMaxUploadDate = DateTime.UtcNow;

    public async Task<List<FlickrPhoto>> GetRecentAsync(int page = 1, int perPage = 10)
    {
        _getRecentMaxUploadDate = DateTime.UtcNow;
        return await GetMoreRecentAsync(page, perPage);
    }

    public async Task<List<FlickrPhoto>> GetMoreRecentAsync(int page = 1, int perPage = 10)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "method", "flickr.photos.search" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "page", page.ToString() },
            { "per_page", perPage.ToString() },
            { "sort", "date-posted-desc" },
            { "max_upload_date", _getRecentMaxUploadDate.ToString(CultureInfo.InvariantCulture) },
            
        };
        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        Debug.WriteLine(requestUrl);
        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.GetRecent>(json);
        return finalResponse != null ? [..finalResponse.Photos.List] : [];
    }

    public async Task<List<FlickrPhoto>> SearchAsync(string text, string tags, int page = 1, int perPage = 10,
        string? sortOrder = null)
    {
        _searchMaxUploadDate = DateTime.UtcNow;
        return await SearchMoreAsync(text, tags, page, perPage, sortOrder);
    }

    public async Task<List<FlickrPhoto>> SearchMoreAsync(string text, string tags, int page = 1, int perPage = 10,
        string? sortOrder = null)
    {
        var queryParams = new Dictionary<string, string>
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
        if (!string.IsNullOrEmpty(tags))
        {
            queryParams.Add("tags", tags);
            queryParams.Add("tag_mode", "all");
        }

        if (!string.IsNullOrEmpty(sortOrder))
        {
            queryParams.Add("sort", sortOrder);
            Debug.WriteLine($"FlickrApiService.SearchMoreAsync: Using sortOrder: {sortOrder}");
        }
        else
        {
            Debug.WriteLine($"FlickrApiService.SearchMoreAsync: No sortOrder provided, using Flickr API default.");
        }

        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        Debug.WriteLine(requestUrl);
        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.Search>(json);
        return finalResponse != null ? [..finalResponse.Photos.List] : [];
    }

    public async Task<List<FlickrPhoto>> GetForLocationAsync(string latitude, string longitude, int page = 1,
        int perPage = 10)
    {
        _getForLocationMaxUploadDate = DateTime.UtcNow;
        return await GetMoreForLocationAsync(latitude, longitude, page, perPage);
    }

    public async Task<List<FlickrPhoto>> GetMoreForLocationAsync(string latitude, string longitude, int page = 1,
        int perPage = 10)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "method", "flickr.photos.search" },
            { "format", "json" },
            { "nojsoncallback", "1" },
            { "api_key", apiKey },
            { "page", page.ToString() },
            { "per_page", perPage.ToString() },
            { "lat", latitude },
            { "lon", longitude },
            { "max_upload_date", _getForLocationMaxUploadDate.ToString(CultureInfo.InvariantCulture) }
        };
        var requestUrl = QueryHelpers.AddQueryString(baseUrl, queryParams);
        Debug.WriteLine(requestUrl);
        var response = await httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.Search>(json);
        return finalResponse != null ? [..finalResponse.Photos.List] : [];
    }

    public async Task<FlickrDetails?> GetDetailsAsync(string photoId)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "method", "flickr.photos.getInfo" },
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
        var finalResponse = JsonSerializer.Deserialize<FlickrApiResponses.GetDetails>(json);
        return finalResponse?.Details;
    }

    public async Task<List<FlickrComment>> GetCommentsAsync(string photoId)
    {
        var queryParams = new Dictionary<string, string>
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
        return finalResponse?.Comments?.List != null ? [..finalResponse.Comments.List] : [];
    }
}