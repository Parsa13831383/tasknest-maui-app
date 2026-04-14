namespace TaskNest.Models.Dashboard;

public sealed class DashboardSummaryDto
{
    public bool IsAuthenticated { get; init; }
    public string DisplayName { get; init; } = "User";
    public int TotalActiveTasks { get; init; }
    public int TasksDueToday { get; init; }
    public int CompletedTaskCount { get; init; }
    public int CompletedThisWeekCount { get; init; }
    public int CategoryCount { get; init; }
    public string CategoryPreviewText { get; init; } = "No categories yet";
    public IReadOnlyList<DashboardFocusItemDto> FocusTasks { get; init; } = Array.Empty<DashboardFocusItemDto>();
}