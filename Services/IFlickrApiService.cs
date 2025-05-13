using FlickrApp.Models;

namespace FlickrApp.Services;

public interface IFlickrApiService
{
    Task<List<FlickrPhoto>> GetRecentAsync(int page = 1, int perPage = 10);
    Task<List<FlickrPhoto>> GetMoreRecentAsync(int page = 1, int perPage = 10);

    Task<List<FlickrPhoto>> SearchAsync(string text, string tags, int page = 1, int perPage = 10,
        string? sortOrder = null);

    Task<List<FlickrPhoto>> SearchMoreAsync(string text, string tags, int page = 1, int perPage = 10,
        string? sortOrder = null);

    Task<List<FlickrPhoto>> GetForLocationAsync(string latitude, string longitude, int page = 1, int perPage = 10);
    
    Task<List<FlickrPhoto>> GetMoreForLocationAsync(string latitude, string longitude, int page = 1, int perPage = 10);
   
    Task<FlickrDetails?> GetDetailsAsync(string photoId);
    
    Task<List<FlickrComment>> GetCommentsAsync(string photoId);

    Task<List<FlickrLicense>> GetLicensesAsync();
}