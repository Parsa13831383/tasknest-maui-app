using Microsoft.Maui.Storage;
using TaskNest.Interfaces;

namespace TaskNest.Services.Security;

public class SecureSessionService : ISecureSessionService
{
    private const string AccessTokenKey = "tasknest_access_token";
    private const string UserIdKey = "tasknest_user_id";
    private static bool usePreferencesFallback;

    public async Task SaveSessionAsync(string accessToken, string userId)
    {
        if (await TrySaveToSecureStorageAsync(accessToken, userId))
        {
            usePreferencesFallback = false;
            return;
        }

        usePreferencesFallback = true;
        Preferences.Set(AccessTokenKey, accessToken);
        Preferences.Set(UserIdKey, userId);
    }

    public async Task<(string? AccessToken, string? UserId)> GetSessionAsync()
    {
        if (!usePreferencesFallback)
        {
            try
            {
                var accessToken = await SecureStorage.Default.GetAsync(AccessTokenKey);
                var userId = await SecureStorage.Default.GetAsync(UserIdKey);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    return (accessToken, userId);
                }
            }
            catch
            {
                usePreferencesFallback = true;
            }
        }

        var fallbackAccessToken = Preferences.Get(AccessTokenKey, string.Empty);
        var fallbackUserId = Preferences.Get(UserIdKey, string.Empty);

        return (string.IsNullOrWhiteSpace(fallbackAccessToken) ? null : fallbackAccessToken,
            string.IsNullOrWhiteSpace(fallbackUserId) ? null : fallbackUserId);
    }

    public async Task ClearSessionAsync()
    {
        try
        {
            SecureStorage.Default.Remove(AccessTokenKey);
            SecureStorage.Default.Remove(UserIdKey);
        }
        catch
        {
            usePreferencesFallback = true;
        }

        Preferences.Remove(AccessTokenKey);
        Preferences.Remove(UserIdKey);
        await Task.CompletedTask;
    }

    private static async Task<bool> TrySaveToSecureStorageAsync(string accessToken, string userId)
    {
        try
        {
            await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
            await SecureStorage.Default.SetAsync(UserIdKey, userId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}