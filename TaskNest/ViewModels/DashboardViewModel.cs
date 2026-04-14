using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using TaskNest.Interfaces;
using TaskNest.Models.Dashboard;

namespace TaskNest.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardService dashboardService;

    [ObservableProperty]
    private string welcomeHeading = "WELCOME";

    [ObservableProperty]
    private string welcomeTitle = "TaskNest Dashboard";

    [ObservableProperty]
    private string welcomeSubtitle = "Your productivity snapshot updates in real time.";

    [ObservableProperty]
    private int todayTaskCount;

    [ObservableProperty]
    private string todayCardSubtitle = "0 due today";

    [ObservableProperty]
    private int completedTaskCount;

    [ObservableProperty]
    private string completedSubtitle = "0 this week";

    [ObservableProperty]
    private int categoryCount;

    [ObservableProperty]
    private string openTaskSummary = "0 open tasks";

    [ObservableProperty]
    private string categorySummary = "No categories yet";

    [ObservableProperty]
    private ObservableCollection<FocusItem> focusItems = new();

    [ObservableProperty]
    private string focusEmptyStateText = "No focus tasks yet. Add a task to get started.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public DashboardViewModel(IDashboardService dashboardService)
    {
        this.dashboardService = dashboardService;
        Title = "Dashboard";

        // Reload dashboard immediately whenever a task is marked complete anywhere in the app.
        WeakReferenceMessenger.Default.Register<TaskStatusChangedMessage>(this, (_, _) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!IsBusy)
                {
                    await LoadAsync();
                }
            });
        });
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var summary = await dashboardService.GetDashboardSummaryAsync(focusLimit: 5);
            ApplySummary(summary);
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "Your session has expired. Please sign in again.";
            ApplySummary(new DashboardSummaryDto { IsAuthenticated = false, DisplayName = "User" });
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Could not load dashboard data: {exception.Message}";
            PublishError(ErrorMessage, exception);
            ApplySummary(new DashboardSummaryDto { IsAuthenticated = false, DisplayName = "User" });
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task NewTaskAsync() => NavigateAsync("taskedit");

    [RelayCommand]
    private Task ViewTasksAsync() => NavigateAsync("tasks");

    [RelayCommand]
    private Task ViewCategoriesAsync() => NavigateAsync("categories");

    private void ApplySummary(DashboardSummaryDto summary)
    {
        WelcomeHeading = summary.IsAuthenticated ? "WELCOME BACK" : "WELCOME";
        WelcomeTitle = summary.IsAuthenticated
            ? $"Hello, {summary.DisplayName}"
            : "Welcome to TaskNest";
        WelcomeSubtitle = summary.IsAuthenticated
            ? "Stay on top of your tasks and progress."
            : "Sign in to view your live dashboard data.";

        TodayTaskCount = summary.TotalActiveTasks;
        TodayCardSubtitle = FormatDueTodaySummary(summary.TasksDueToday);
        CompletedTaskCount = summary.CompletedTaskCount;
        CompletedSubtitle = $"{summary.CompletedThisWeekCount} this week";
        CategoryCount = summary.CategoryCount;
        CategorySummary = summary.CategoryPreviewText;
        OpenTaskSummary = FormatOpenTaskSummary(summary.TotalActiveTasks);

        FocusItems.Clear();
        foreach (var focusTask in summary.FocusTasks)
        {
            FocusItems.Add(new FocusItem(
                focusTask.Title,
                focusTask.Description,
                focusTask.DueLabel,
                BuildMetaLabel(focusTask),
                ResolveAccentColor(focusTask.PriorityColorKey)));
        }

        FocusEmptyStateText = summary.IsAuthenticated
            ? "No focus tasks yet. Create a task and it will appear here."
            : "No session found. Please sign in to load your tasks.";
    }

    private static string BuildMetaLabel(DashboardFocusItemDto focusTask)
    {
        return $"{focusTask.CategoryName} / {focusTask.Priority}";
    }

    private static string FormatOpenTaskSummary(int activeTaskCount)
    {
        return activeTaskCount == 1
            ? "1 open task"
            : $"{activeTaskCount} open tasks";
    }

    private static string FormatDueTodaySummary(int dueTodayCount)
    {
        return dueTodayCount == 1
            ? "1 due today"
            : $"{dueTodayCount} due today";
    }

    private static Color ResolveAccentColor(string priorityColorKey)
    {
        return priorityColorKey switch
        {
            "High" => Color.FromArgb("#DC2626"),
            "Medium" => Color.FromArgb("#D97706"),
            _ => Color.FromArgb("#2563EB")
        };
    }
}

public sealed record FocusItem(
    string Title,
    string Subtitle,
    string TimeLabel,
    string MetaLabel,
    Color AccentColor);