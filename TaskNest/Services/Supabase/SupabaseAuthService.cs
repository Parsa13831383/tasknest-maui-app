using System.Text;
using System.Text.Json;
using TaskNest.Interfaces;
using TaskNest.Models.Auth;

namespace TaskNest.Services.Supabase;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly HttpClient _httpClient;

    private string? _accessToken;
    private string? _userId;

    public string? AccessToken => _accessToken;
    public string? UserId => _userId;
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken);

    public SupabaseAuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(SupabaseConfig.SupabaseUrl)
        };

        _httpClient.DefaultRequestHeaders.Add("apikey", SupabaseConfig.SupabaseAnonKey);
    }

    public async Task<AuthResponse?> SignUpAsync(string email, string password)
    {
        var payload = new SignUpRequest
        {
            Email = email,
            Password = password
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/auth/v1/signup", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Sign-up failed: {responseBody}");
        }

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        SetSession(authResponse);

        return authResponse;
    }

    public async Task<AuthResponse?> SignInAsync(string email, string password)
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
            throw new Exception($"Sign-in failed: {responseBody}");
        }

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        SetSession(authResponse);

        return authResponse;
    }

    public Task SignOutAsync()
    {
        _accessToken = null;
        _userId = null;
        return Task.CompletedTask;
    }

    private void SetSession(AuthResponse? authResponse)
    {
        _accessToken = authResponse?.AccessToken;
        _userId = authResponse?.User?.Id;
    }
}