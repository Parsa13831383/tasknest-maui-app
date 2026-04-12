namespace TaskNest.Interfaces;

public interface ISecureSessionService
{
    Task SaveSessionAsync(string accessToken, string userId);
    Task<(string? AccessToken, string? UserId)> GetSessionAsync();
    Task ClearSessionAsync();
}