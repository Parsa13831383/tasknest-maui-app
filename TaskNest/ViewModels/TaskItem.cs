namespace TaskNest.ViewModels;

public class TaskItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string DueDate { get; set; } = "";
    public string Category { get; set; } = "";

    public string PriorityText { get; set; } = "";
    public Color PriorityColor { get; set; } = Colors.Gray;
}