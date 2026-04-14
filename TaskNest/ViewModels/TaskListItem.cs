using Microsoft.Maui.Graphics;

namespace TaskNest.ViewModels;

public class TaskListItem
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DueDate { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public Color TaskColor { get; set; } = Colors.Gray;

    public bool IsCompleted { get; set; }
}