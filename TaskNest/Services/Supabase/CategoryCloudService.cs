using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskNest.Interfaces;
using TaskNest.Models.Cloud;

namespace TaskNest.Services.Supabase;

public class CategoryCloudService : ICategoryCloudService
{
    private readonly ISupabaseAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CategoryCloudService(ISupabaseAuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(SupabaseConfig.SupabaseUrl)
        };
    }

    private void ApplyHeaders(bool returnRepresentation = false)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", SupabaseConfig.SupabaseAnonKey);

        if (!string.IsNullOrWhiteSpace(_authService.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
        }

        if (returnRepresentation)
        {
            _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
        }
    }

    public async Task<List<CloudCategoryDto>> GetCategoriesAsync()
    {
        ApplyHeaders();

        var response = await _httpClient.GetAsync(
            "/rest/v1/categories?select=*&is_deleted=eq.false&order=created_at.desc");

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Get categories failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudCategoryDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<CloudCategoryDto?> GetCategoryByIdAsync(string id)
    {
        ApplyHeaders();

        var response = await _httpClient.GetAsync(
            $"/rest/v1/categories?select=*&id=eq.{id}&is_deleted=eq.false");

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Get category failed: {body}");
        }

        var items = JsonSerializer.Deserialize<List<CloudCategoryDto>>(body, _jsonOptions) ?? new();
        return items.FirstOrDefault();
    }

    public async Task<List<CloudCategoryDto>> CreateCategoryAsync(CloudCategoryDto category)
    {
        ApplyHeaders(returnRepresentation: true);

        var now = DateTime.UtcNow;
        var payload = new Dictionary<string, object?>
        {
            ["name"] = category.Name,
            ["description"] = category.Description,
            ["created_at"] = now,
            ["updated_at"] = now,
            ["is_deleted"] = false
        };

        if (!string.IsNullOrWhiteSpace(category.UserId))
        {
            payload["user_id"] = category.UserId;
        }

        var json = JsonSerializer.Serialize(new[] { payload });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/rest/v1/categories", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Create category failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudCategoryDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<List<CloudCategoryDto>> UpdateCategoryAsync(string id, CloudCategoryDto category)
    {
        ApplyHeaders(returnRepresentation: true);

        var payload = new
        {
            name = category.Name,
            description = category.Description,
            updated_at = DateTime.UtcNow,
            sync_status = "synced"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/rest/v1/categories?id=eq.{id}", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Update category failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudCategoryDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<List<CloudCategoryDto>> SoftDeleteCategoryAsync(string id)
    {
        ApplyHeaders(returnRepresentation: true);

        var payload = new
        {
            is_deleted = true,
            updated_at = DateTime.UtcNow,
            sync_status = "synced"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/rest/v1/categories?id=eq.{id}", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Delete category failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudCategoryDto>>(body, _jsonOptions) ?? new();
    }
}