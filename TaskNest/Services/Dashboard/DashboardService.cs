using TaskNest.Interfaces;
using TaskNest.Models;
using TaskNest.Models.Auth;
using TaskNest.Models.Dashboard;

namespace TaskNest.Services.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly ISupabaseAuthService authService;
    private readonly IUnitOfWork unitOfWork;

    public DashboardService(ISupabaseAuthService authService, IUnitOfWork unitOfWork)
    {
        this.authService = authService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int focusLimit = 5, CancellationToken cancellationToken = default)
    {
        focusLimit = Math.Clamp(focusLimit, 1, 10);

        var isAuthenticated = authService.IsAuthenticated;
        if (!isAuthenticated)
        {
            isAuthenticated = await authService.RestoreSessionAsync();
        }

        if (!isAuthenticated)
        {
            return new DashboardSummaryDto
            {
                IsAuthenticated = false,
                DisplayName = "User"
            };
        }

        // Resolve display name from the cached session info BEFORE making any
        // network call that could inadvertently clear the access token.
        var displayName = ResolveDisplayName(null, authService.UserEmail);

        // Fetch user profile in the background for a nicer greeting, but never
        // let a failure here break the dashboard or clear the session token.
        try
        {
            var user = await authService.GetCurrentUserAsync();
            if (user is not null)
            {
                displayName = ResolveDisplayName(user, authService.UserEmail);
            }
        }
        catch
        {
            // Profile fetch failed — continue with the email-based display name.
        }

        // If GetCurrentUserAsync cleared the session (e.g. 401), restore it
        // so subsequent data calls still carry the user's JWT.
        if (!authService.IsAuthenticated)
        {
            await authService.RestoreSessionAsync();
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tasks = await unitOfWork.Tasks.GetAllAsync();
        var categories = await unitOfWork.Categories.GetAllAsync();

        cancellationToken.ThrowIfCancellationRequested();

        var activeTasks = tasks
            .Where(task => !task.IsDeleted && !task.IsCompleted)
            .ToList();

        var completedTasks = tasks
            .Where(task => !task.IsDeleted && task.IsCompleted)
            .ToList();

        var visibleCategories = categories
            .Where(category => !category.IsDeleted)
            .ToList();

        var today = DateTime.Today;
        var startOfWeek = GetStartOfWeek(today);
        var weekStartUtc = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);

        var dueTodayCount = activeTasks.Count(task => task.DueDate?.Date == today);
        var completedThisWeek = completedTasks.Count(task => task.UpdatedAtUtc >= weekStartUtc);

        var categoriesById = visibleCategories.ToDictionary(category => category.Id, category => category.Name);

        var topCategories = visibleCategories
            .OrderByDescending(category => activeTasks.Count(task => task.CategoryId == category.Id))
            .ThenBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .Select(category => category.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        var categoryPreview = topCategories.Count > 0
            ? string.Join(", ", topCategories)
            : "No categories yet";

        var focusTasks = BuildFocusItems(activeTasks, categoriesById, today, focusLimit);
        var weeklyCompleted = BuildWeeklyCompleted(completedTasks, startOfWeek, today);

        return new DashboardSummaryDto
        {
            IsAuthenticated = true,
            DisplayName = displayName,
            TotalActiveTasks = activeTasks.Count,
            TasksDueToday = dueTodayCount,
            CompletedTaskCount = completedTasks.Count,
            CompletedThisWeekCount = completedThisWeek,
            CategoryCount = visibleCategories.Count,
            CategoryPreviewText = categoryPreview,
            FocusTasks = focusTasks,
            WeeklyCompleted = weeklyCompleted
        };
    }

    private static IReadOnlyList<DashboardFocusItemDto> BuildFocusItems(
        IEnumerable<TaskItem> activeTasks,
        IReadOnlyDictionary<string, string> categoriesById,
        DateTime today,
        int focusLimit)
    {
        return activeTasks
            .OrderBy(task => GetFocusSortRank(task, today))
            .ThenBy(task => task.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(task => task.UpdatedAtUtc)
            .Take(focusLimit)
            .Select(task => MapFocusItem(task, categoriesById, today))
            .ToList();
    }

    private static DashboardFocusItemDto MapFocusItem(
        TaskItem task,
        IReadOnlyDictionary<string, string> categoriesById,
        DateTime today)
    {
        var dueDate = task.DueDate?.Date;
        var isDueToday = dueDate == today;
        var isOverdue = dueDate.HasValue && dueDate.Value < today;

        var priorityLabel = ResolvePriorityLabel(dueDate, today);

        return new DashboardFocusItemDto
        {
            Title = task.Title,
            Description = string.IsNullOrWhiteSpace(task.Description) ? "No description" : task.Description,
            DueLabel = BuildDueLabel(dueDate, today),
            Priority = priorityLabel,
            PriorityColorKey = ResolvePriorityColorKey(priorityLabel),
            CategoryName = ResolveCategoryName(task.CategoryId, categoriesById),
            IsDueToday = isDueToday,
            IsOverdue = isOverdue
        };
    }

    private static int GetFocusSortRank(TaskItem task, DateTime today)
    {
        var dueDate = task.DueDate?.Date;

        if (dueDate == today)
        {
            return 0;
        }

        if (dueDate.HasValue && dueDate.Value < today)
        {
            return 1;
        }

        if (dueDate.HasValue)
        {
            return 2;
        }

        return 3;
    }

    private static string ResolveCategoryName(string? categoryId, IReadOnlyDictionary<string, string> categoriesById)
    {
        if (!string.IsNullOrWhiteSpace(categoryId) && categoriesById.TryGetValue(categoryId, out var categoryName))
        {
            return categoryName;
        }

        return "Uncategorized";
    }

    private static string BuildDueLabel(DateTime? dueDate, DateTime today)
    {
        if (!dueDate.HasValue)
        {
            return "No due date";
        }

        if (dueDate.Value == today)
        {
            return "Today";
        }

        if (dueDate.Value == today.AddDays(1))
        {
            return "Tomorrow";
        }

        if (dueDate.Value < today)
        {
            return $"Overdue since {dueDate.Value:dd MMM}";
        }

        return dueDate.Value.ToString("dd MMM");
    }

    private static string ResolvePriorityLabel(DateTime? dueDate, DateTime today)
    {
        if (!dueDate.HasValue)
        {
            return "Normal";
        }

        if (dueDate.Value <= today)
        {
            return "High";
        }

        if (dueDate.Value <= today.AddDays(3))
        {
            return "Medium";
        }

        return "Normal";
    }

    private static string ResolvePriorityColorKey(string priorityLabel)
    {
        return priorityLabel switch
        {
            "High" => "High",
            "Medium" => "Medium",
            _ => "Normal"
        };
    }

    private static string ResolveDisplayName(AuthenticatedUserInfo? user, string? fallbackEmail)
    {
        if (!string.IsNullOrWhiteSpace(user?.FullName))
        {
            return user.FullName.Trim();
        }

        var email = !string.IsNullOrWhiteSpace(user?.Email) ? user.Email : fallbackEmail;
        if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
        {
            return email.Split('@')[0].Trim();
        }

        return "User";
    }

    private static DateTime GetStartOfWeek(DateTime value)
    {
        var diff = (7 + (value.DayOfWeek - DayOfWeek.Monday)) % 7;
        return value.AddDays(-1 * diff).Date;
    }

    private static IReadOnlyList<DailyCompletedDto> BuildWeeklyCompleted(
        IReadOnlyCollection<TaskItem> completedTasks,
        DateTime startOfWeek,
        DateTime today)
    {
        var dayLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var result = new List<DailyCompletedDto>(7);

        for (var i = 0; i < 7; i++)
        {
            var day = startOfWeek.AddDays(i);
            var dayUtcStart = DateTime.SpecifyKind(day, DateTimeKind.Utc);
            var dayUtcEnd = dayUtcStart.AddDays(1);

            var count = completedTasks.Count(t =>
                t.UpdatedAtUtc >= dayUtcStart && t.UpdatedAtUtc < dayUtcEnd);

            result.Add(new DailyCompletedDto
            {
                DayLabel = dayLabels[i],
                Count = count,
                IsToday = day.Date == today
            });
        }

        return result;
    }
}