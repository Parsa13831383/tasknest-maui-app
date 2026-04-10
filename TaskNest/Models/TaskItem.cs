using SQLite;
using Microsoft.Maui.Graphics;

namespace TaskNest.Models;

public class TaskItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(150)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    public DateTime? DueDate { get; set; }

    public string Priority { get; set; } = "Low";

    public bool IsCompleted { get; set; }

    public int? CategoryId { get; set; }

    // Sync-ready fields (HIGH MARKS)
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    //DO NOT STORE IN DB
    [Ignore]
    public Color PriorityColor => Priority switch
    {
        "High" => Colors.Red,
        "Medium" => Colors.Orange,
        "Low" => Colors.Green,
        _ => Colors.Gray
    };
}