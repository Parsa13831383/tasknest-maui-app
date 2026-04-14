using TaskNest.Models.Auth;

namespace TaskNest.Interfaces;

public interface ISupabaseAuthService
{
    event EventHandler<string>? SessionExpired;

    Task<AuthResponse?> SignUpAsync(string email, string password, string? fullName = null);
    Task<AuthResponse?> SignInAsync(string email, string password, bool rememberSession = true);
    Task SendPasswordResetEmailAsync(string email);
    Task ApplyRecoverySessionAsync(string accessToken);
    Task UpdatePasswordAsync(string newPassword);
    Task<AuthenticatedUserInfo?> GetCurrentUserAsync();
    Task SignOutAsync();
    Task<bool> RestoreSessionAsync();
    Task HandleSessionExpiredAsync(string reason = "Session expired");

    string? AccessToken { get; }
    string? UserId { get; }
    string? UserEmail { get; }
    bool IsAuthenticated { get; }
}