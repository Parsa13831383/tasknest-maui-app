using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TaskNest.Interfaces;
using TaskNest.Models.Cloud;

namespace TaskNest.Services.Supabase;

// Handles cloud communication with Supabase REST API for task data.
public class TaskCloudService : ITaskCloudService
{
    private readonly ISupabaseAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TaskCloudService(ISupabaseAuthService authService)
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

    public async Task<List<CloudTaskDto>> GetTasksAsync()
    {
        ApplyHeaders();

        var response = await _httpClient.GetAsync(
            "/rest/v1/tasks?select=*&is_deleted=eq.false&order=created_at.desc");

        var body = await response.Content.ReadAsStringAsync();

        await HandleUnauthorizedAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Get tasks failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudTaskDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<CloudTaskDto?> GetTaskByIdAsync(string id)
    {
        ApplyHeaders();

        var response = await _httpClient.GetAsync(
            $"/rest/v1/tasks?select=*&id=eq.{id}&is_deleted=eq.false");

        var body = await response.Content.ReadAsStringAsync();

        await HandleUnauthorizedAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Get task failed: {body}");
        }

        var items = JsonSerializer.Deserialize<List<CloudTaskDto>>(body, _jsonOptions) ?? new();
        return items.FirstOrDefault();
    }

    public async Task<List<CloudTaskDto>> CreateTaskAsync(CloudTaskDto task)
    {
        ApplyHeaders(returnRepresentation: true);

        var now = DateTime.UtcNow;
        var payload = new Dictionary<string, object?>
        {
            ["title"] = task.Title,
            ["description"] = task.Description,
            ["due_date"] = task.DueDate,
            ["is_completed"] = task.IsCompleted,
            ["category_id"] = task.CategoryId,
            ["created_at"] = now,
            ["updated_at"] = now,
            ["is_deleted"] = false
        };

        if (!string.IsNullOrWhiteSpace(task.UserId))
        {
            payload["user_id"] = task.UserId;
        }

        var json = JsonSerializer.Serialize(new[] { payload });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/rest/v1/tasks", content);
        var body = await response.Content.ReadAsStringAsync();

        await HandleUnauthorizedAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Create task failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudTaskDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<List<CloudTaskDto>> UpdateTaskAsync(string id, CloudTaskDto task)
    {
        ApplyHeaders(returnRepresentation: true);

        task.UpdatedAtUtc = DateTime.UtcNow;

        var payload = new
        {
            title = task.Title,
            description = task.Description,
            due_date = task.DueDate,
            is_completed = task.IsCompleted,
            category_id = task.CategoryId,
            updated_at = task.UpdatedAtUtc
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/rest/v1/tasks?id=eq.{id}", content);
        var body = await response.Content.ReadAsStringAsync();

        await HandleUnauthorizedAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Update task failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudTaskDto>>(body, _jsonOptions) ?? new();
    }

    public async Task<List<CloudTaskDto>> SoftDeleteTaskAsync(string id)
    {
        ApplyHeaders(returnRepresentation: true);

        var payload = new
        {
            is_deleted = true,
            updated_at = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/rest/v1/tasks?id=eq.{id}", content);
        var body = await response.Content.ReadAsStringAsync();

        await HandleUnauthorizedAsync(response);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Delete task failed: {body}");
        }

        return JsonSerializer.Deserialize<List<CloudTaskDto>>(body, _jsonOptions) ?? new();
    }

    private async Task HandleUnauthorizedAsync(HttpResponseMessage response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            return;
        }

        await _authService.HandleSessionExpiredAsync();
        throw new UnauthorizedAccessException("Session expired");
    }
}