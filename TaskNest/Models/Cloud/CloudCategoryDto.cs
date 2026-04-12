using System.Text.Json.Serialization;

namespace TaskNest.Models.Cloud;

public class CloudCategoryDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAtUtc { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAtUtc { get; set; }

    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("sync_status")]
    public string SyncStatus { get; set; } = "synced";
}