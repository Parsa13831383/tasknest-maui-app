using TaskNest.Models.Auth;

namespace TaskNest.Interfaces;

public interface ISupabaseAuthService
{
    Task<AuthResponse?> SignUpAsync(string email, string password, string? fullName = null);
    Task<AuthResponse?> SignInAsync(string email, string password, bool rememberSession = true);
    Task SignOutAsync();
    bool TryRestoreSession();

    string? AccessToken { get; }
    string? UserId { get; }
    bool IsAuthenticated { get; }
}