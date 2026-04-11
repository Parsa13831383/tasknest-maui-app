using Microsoft.Maui.Graphics;

namespace TaskNest.ViewModels;

public class TaskListItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DueDate { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string PriorityText { get; set; } = string.Empty;

    public Color PriorityColor { get; set; } = Colors.Gray;
}