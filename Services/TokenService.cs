using System.Diagnostics;

namespace FlickrApp.Services;

public class TokenService : ITokenService
{
    public const string OAuthTokenKey = "oauth_token";
    public const string RefreshTokenKey = "refresh_token";

    public async Task<bool> SaveOAuthTokenAsync(string token, string refrehToken)
    {
        try
        {
            await SecureStorage.Default.SetAsync(OAuthTokenKey, token);
            await SecureStorage.Default.SetAsync(RefreshTokenKey, refrehToken);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public async Task<bool> SavePersistentTokenAsync(string key, string token)
    {
        try
        {
            await SecureStorage.Default.SetAsync(key, token);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public async Task<string?> GetOAuthTokenAsync()
    {
        var token = await SecureStorage.Default.GetAsync(OAuthTokenKey);
        return token;
    }

    public Task<string?> GetPersistentTokenAsync(string key)
    {
        var token = SecureStorage.Default.GetAsync(key);
        return token;
    }

    public Task<bool> RefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public bool RemovePersistentToken(string key)
    {
        SecureStorage.Default.Remove(key);
        return true;
    }

    public bool RemoveOAuthToken()
    {
        SecureStorage.Default.Remove(OAuthTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        return true;
    }
}