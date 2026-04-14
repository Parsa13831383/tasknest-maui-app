using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;
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
    private string? _userEmail;

    public event EventHandler<string>? SessionExpired;

    public string? AccessToken => _accessToken;
    public string? UserId => _userId;
    public string? UserEmail => _userEmail;
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

    public async Task SendPasswordResetEmailAsync(string email)
    {
        var redirectUrl = string.IsNullOrWhiteSpace(SupabaseConfig.PasswordResetRedirectUrl)
            ? null
            : SupabaseConfig.PasswordResetRedirectUrl;

        var payload = redirectUrl is null
            ? JsonSerializer.Serialize(new { email })
            : JsonSerializer.Serialize(new { email, redirect_to = redirectUrl });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/auth/v1/recover", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetSupabaseErrorMessage(responseBody, "Could not send password reset email."));
        }
    }

    public async Task ApplyRecoverySessionAsync(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new Exception("Password recovery token is missing.");
        }

        _accessToken = accessToken;
        _userId = TryExtractUserIdFromJwt(accessToken);
        _userEmail = TryExtractJwtStringClaim(accessToken, "email");

        await _secureSessionService.SaveSessionAsync(_accessToken, _userId ?? string.Empty);
    }

    public async Task UpdatePasswordAsync(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new Exception("Your password reset session has expired. Please request a new reset email.");
        }

        var payload = JsonSerializer.Serialize(new
        {
            password = newPassword
        });

        using var request = new HttpRequestMessage(HttpMethod.Put, "/auth/v1/user");
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetSupabaseErrorMessage(responseBody, "Could not update password."));
        }
    }

    public async Task<AuthenticatedUserInfo?> GetCurrentUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "/auth/v1/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HandleSessionExpiredAsync();
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetSupabaseErrorMessage(responseBody, "Could not load profile information."));
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return BuildUserFromToken();
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            var id = root.TryGetProperty("id", out var idElement) && idElement.ValueKind == JsonValueKind.String
                ? idElement.GetString() ?? string.Empty
                : string.Empty;

            var email = root.TryGetProperty("email", out var emailElement) && emailElement.ValueKind == JsonValueKind.String
                ? emailElement.GetString() ?? string.Empty
                : string.Empty;

            string fullName = string.Empty;
            if (root.TryGetProperty("user_metadata", out var metadataElement)
                && metadataElement.ValueKind == JsonValueKind.Object)
            {
                fullName = TryGetMetadataField(metadataElement, "full_name")
                    ?? TryGetMetadataField(metadataElement, "name")
                    ?? TryGetMetadataField(metadataElement, "display_name")
                    ?? string.Empty;
            }

            var jwtUser = BuildUserFromToken();

            _userId = !string.IsNullOrWhiteSpace(id) ? id : jwtUser?.Id ?? _userId;
            _userEmail = !string.IsNullOrWhiteSpace(email) ? email : jwtUser?.Email ?? _userEmail;

            return new AuthenticatedUserInfo
            {
                Id = !string.IsNullOrWhiteSpace(id) ? id : jwtUser?.Id ?? _userId ?? string.Empty,
                Email = !string.IsNullOrWhiteSpace(email) ? email : jwtUser?.Email ?? string.Empty,
                FullName = !string.IsNullOrWhiteSpace(fullName) ? fullName : jwtUser?.FullName ?? string.Empty
            };
        }
        catch (JsonException)
        {
            return BuildUserFromToken();
        }
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
        _userEmail = TryExtractJwtStringClaim(token, "email");

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
        _userEmail = authResponse?.User?.Email;

        if (string.IsNullOrWhiteSpace(_userId) && !string.IsNullOrWhiteSpace(_accessToken))
        {
            _userId = TryExtractUserIdFromJwt(_accessToken);
        }

        if (string.IsNullOrWhiteSpace(_userEmail) && !string.IsNullOrWhiteSpace(_accessToken))
        {
            _userEmail = TryExtractJwtStringClaim(_accessToken, "email");
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
        _userEmail = null;
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
        return TryExtractJwtStringClaim(jwt, "sub");
    }

    private AuthenticatedUserInfo? BuildUserFromToken()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            return null;
        }

        var id = TryExtractJwtStringClaim(_accessToken, "sub") ?? _userId ?? string.Empty;
        var email = TryExtractJwtStringClaim(_accessToken, "email") ?? string.Empty;
        var fullName = TryExtractJwtMetadataClaim(_accessToken, "full_name")
            ?? TryExtractJwtMetadataClaim(_accessToken, "name")
            ?? string.Empty;

        return new AuthenticatedUserInfo
        {
            Id = id,
            Email = email,
            FullName = fullName
        };
    }

    private static string? TryGetMetadataField(JsonElement metadataElement, string fieldName)
    {
        if (metadataElement.TryGetProperty(fieldName, out var field)
            && field.ValueKind == JsonValueKind.String)
        {
            return field.GetString();
        }

        return null;
    }

    private static string? TryExtractJwtMetadataClaim(string jwt, string claimName)
    {
        var payload = TryDecodeJwtPayload(jwt);
        if (payload is null)
        {
            return null;
        }

        using var document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("user_metadata", out var metadata)
            || metadata.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return TryGetMetadataField(metadata, claimName);
    }

    private static string? TryExtractJwtStringClaim(string jwt, string claimName)
    {
        var payload = TryDecodeJwtPayload(jwt);
        if (payload is null)
        {
            return null;
        }

        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.TryGetProperty(claimName, out var value)
            && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }

    private static string? TryDecodeJwtPayload(string jwt)
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
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            // Ignore invalid JWT payload format and leave user id empty.
        }

        return null;
    }
}