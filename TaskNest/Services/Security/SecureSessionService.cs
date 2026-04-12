using Microsoft.Maui.Storage;
using TaskNest.Interfaces;

namespace TaskNest.Services.Security;

public class SecureSessionService : ISecureSessionService
{
    private const string AccessTokenKey = "tasknest_access_token";
    private const string UserIdKey = "tasknest_user_id";

    public async Task SaveSessionAsync(string accessToken, string userId)
    {
        await SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.Default.SetAsync(UserIdKey, userId);
    }

    public async Task<(string? AccessToken, string? UserId)> GetSessionAsync()
    {
        var accessToken = await SecureStorage.Default.GetAsync(AccessTokenKey);
        var userId = await SecureStorage.Default.GetAsync(UserIdKey);

        return (accessToken, userId);
    }

    public async Task ClearSessionAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(UserIdKey);
        await Task.CompletedTask;
    }
}