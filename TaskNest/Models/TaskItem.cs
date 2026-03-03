using Microsoft.Maui.Graphics;

namespace TaskNest.Models;

public class TaskItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Low";
    public bool IsCompleted { get; set; }

    public Color PriorityColor => Priority switch
    {
        "High" => Colors.Red,
        "Medium" => Colors.Orange,
        "Low" => Colors.Green,
        _ => Colors.Gray
    };
}