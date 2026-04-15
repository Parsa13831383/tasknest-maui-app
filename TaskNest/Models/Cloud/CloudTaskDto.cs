using System.Text.Json.Serialization;

namespace TaskNest.Models.Cloud;

public class CloudTaskDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("reflection")]
    public string Reflection { get; set; } = string.Empty;

    [JsonPropertyName("due_date")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "Low";

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAtUtc { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAtUtc { get; set; }

    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("sync_status")]
    public string SyncStatus { get; set; } = "synced";
}