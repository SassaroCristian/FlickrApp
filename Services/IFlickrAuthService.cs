namespace FlickrApp.Services;

public interface IFlickrAuthService
{
    Task<bool> GetOauthTokenAsync();
    Task<bool> AuthenticateAsync();
}