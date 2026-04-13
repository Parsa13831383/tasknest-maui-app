using System.Text;
using System.Text.Json;
using TaskNest.Interfaces;
using TaskNest.Models.Auth;

namespace TaskNest.Services.Supabase;

// Handles authentication lifecycle and Supabase auth API interactions.
public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly ISecureSessionService _secureSessionService;
    private readonly HttpClient _httpClient;

    private string? _accessToken;
    private string? _userId;

    public event EventHandler<string>? SessionExpired;

    public string? AccessToken => _accessToken;
    public string? UserId => _userId;
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken);

    public SupabaseAuthService(ISecureSessionService secureSessionService)
    {
        _secureSessionService = secureSessionService;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(SupabaseConfig.SupabaseUrl)
        };

        _httpClient.DefaultRequestHeaders.Add("apikey", SupabaseConfig.SupabaseAnonKey);
    }

    public async Task<AuthResponse?> SignUpAsync(string email, string password, string? fullName = null)
    {
        var redirectUrl = string.IsNullOrWhiteSpace(SupabaseConfig.EmailConfirmationRedirectUrl)
            ? null
            : SupabaseConfig.EmailConfirmationRedirectUrl;

        var payload = new SignUpRequest
        {
            Email = email,
            Password = password,
            EmailRedirectTo = redirectUrl,
            Data = new SignUpMetadata
            {
                FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim(),
                AppName = SupabaseConfig.AppName
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/v1/signup", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var message = GetSupabaseErrorMessage(responseBody, "Sign-up failed.");

            if (IsAlreadyRegisteredMessage(message)
                && await TryResendSignupConfirmationAsync(email))
            {
                throw new Exception("This email is already registered but not yet confirmed. A new confirmation email has been sent.");
            }

            throw new Exception(message);
        }

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await SetSessionAsync(authResponse);

        return authResponse;
    }

    private async Task<bool> TryResendSignupConfirmationAsync(string email)
    {
        var payload = JsonSerializer.Serialize(new
        {
            type = "signup",
            email
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/auth/v1/resend", content);
        return response.IsSuccessStatusCode;
    }

    private static bool IsAlreadyRegisteredMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        return message.Contains("already", StringComparison.OrdinalIgnoreCase)
            || message.Contains("registered", StringComparison.OrdinalIgnoreCase)
            || message.Contains("exists", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<AuthResponse?> SignInAsync(string email, string password, bool rememberSession = true)
    {
        var payload = new SignInRequest
        {
            Email = email,
            Password = password
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/v1/token?grant_type=password", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetSupabaseErrorMessage(responseBody, "Sign-in failed."));
        }

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await SetSessionAsync(authResponse, rememberSession);

        return authResponse;
    }

    public async Task SignOutAsync()
    {
        await ClearSessionAsync();
    }

    public async Task HandleSessionExpiredAsync(string reason = "Session expired")
    {
        await ClearSessionAsync();
        SessionExpired?.Invoke(this, reason);
    }

    public async Task<bool> RestoreSessionAsync()
    {
        var session = await _secureSessionService.GetSessionAsync();
        var token = session.AccessToken;

        if (string.IsNullOrWhiteSpace(token))
        {
            await ClearSessionAsync();
            return false;
        }

        _accessToken = token;
        _userId = session.UserId;

        if (string.IsNullOrWhiteSpace(_userId))
        {
            _userId = TryExtractUserIdFromJwt(token);
            await _secureSessionService.SaveSessionAsync(_accessToken, _userId ?? string.Empty);
        }

        return true;
    }

    private async Task SetSessionAsync(AuthResponse? authResponse, bool persistSession = true)
    {
        _accessToken = authResponse?.AccessToken;
        _userId = authResponse?.User?.Id;

        if (string.IsNullOrWhiteSpace(_userId) && !string.IsNullOrWhiteSpace(_accessToken))
        {
            _userId = TryExtractUserIdFromJwt(_accessToken);
        }

        if (!persistSession)
        {
            await _secureSessionService.ClearSessionAsync();
            return;
        }

        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            await _secureSessionService.ClearSessionAsync();
            return;
        }

        await _secureSessionService.SaveSessionAsync(_accessToken, _userId ?? string.Empty);
    }

    private async Task ClearSessionAsync()
    {
        _accessToken = null;
        _userId = null;
        await _secureSessionService.ClearSessionAsync();
    }

    private static string GetSupabaseErrorMessage(string responseBody, string fallbackMessage)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return fallbackMessage;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.TryGetProperty("msg", out var msg) && msg.ValueKind == JsonValueKind.String)
            {
                return msg.GetString() ?? fallbackMessage;
            }

            if (root.TryGetProperty("error_description", out var desc) && desc.ValueKind == JsonValueKind.String)
            {
                return desc.GetString() ?? fallbackMessage;
            }

            if (root.TryGetProperty("message", out var message) && message.ValueKind == JsonValueKind.String)
            {
                return message.GetString() ?? fallbackMessage;
            }
        }
        catch (JsonException)
        {
            // Ignore parse errors and fall back to a generic message.
        }

        return fallbackMessage;
    }

    private static string? TryExtractUserIdFromJwt(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
        {
            return null;
        }

        var payload = parts[1]
            .Replace('-', '+')
            .Replace('_', '/');

        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;
            case 3:
                payload += "=";
                break;
        }

        try
        {
            var bytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(bytes);
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("sub", out var sub)
                && sub.ValueKind == JsonValueKind.String)
            {
                return sub.GetString();
            }
        }
        catch
        {
            // Ignore invalid JWT payload format and leave user id empty.
        }

        return null;
    }
}