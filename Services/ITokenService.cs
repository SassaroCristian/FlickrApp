namespace FlickrApp.Services;

public interface ITokenService
{
    Task<bool> SaveOAuthTokenAsync(string key, string token);
    Task<bool> SavePersistentTokenAsync(string key, string token);

    Task<string?> GetOAuthTokenAsync();
    Task<string?> GetPersistentTokenAsync(string key);

    Task<bool> RefreshTokenAsync();

    bool RemovePersistentToken(string key);
    bool RemoveOAuthToken();
}