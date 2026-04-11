using TaskNest.Models.Auth;

namespace TaskNest.Interfaces;

public interface ISupabaseAuthService
{
    Task<AuthResponse?> SignUpAsync(string email, string password);
    Task<AuthResponse?> SignInAsync(string email, string password);
    Task SignOutAsync();

    string? AccessToken { get; }
    string? UserId { get; }
    bool IsAuthenticated { get; }
}