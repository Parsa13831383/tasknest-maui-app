using SQLite;
using Microsoft.Maui.Graphics;
using TaskNest.Models.Enums;

namespace TaskNest.Models;

public class TaskItem
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    [MaxLength(2000)]
    public string Reflection { get; set; } = "";

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public string? CategoryId { get; set; }

    [MaxLength(32)]
    public string? TaskColorHex { get; set; }

    // Sync-ready fields (HIGH MARKS)
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingCreate;

    //DO NOT STORE IN DB
    [Ignore]
    public Color TaskColor
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(TaskColorHex) && Color.TryParse(TaskColorHex.Trim(), out var customColor))
            {
                return customColor;
            }

            return Colors.Gray;
        }
    }
}