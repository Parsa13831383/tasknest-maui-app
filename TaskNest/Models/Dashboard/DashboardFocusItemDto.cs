namespace TaskNest.Models.Dashboard;

public sealed class DashboardFocusItemDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DueLabel { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string PriorityColorKey { get; init; } = "Neutral";
    public string CategoryName { get; init; } = string.Empty;
    public bool IsDueToday { get; init; }
    public bool IsOverdue { get; init; }
}