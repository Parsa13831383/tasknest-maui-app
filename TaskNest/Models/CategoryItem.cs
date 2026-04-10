using SQLite;
using Microsoft.Maui.Graphics;

namespace TaskNest.Models;

public class CategoryItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(100), Unique]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Sync-ready fields
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    // ❌ UI ONLY (ignored in DB)
    [Ignore]
    public string TaskCountText { get; set; } = string.Empty;

    [Ignore]
    public Color BadgeBackgroundColor { get; set; } = Colors.LightGray;

    [Ignore]
    public Color BadgeTextColor { get; set; } = Colors.Black;

    [Ignore]
    public int Count { get; set; }
}