using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TaskNest.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private int todayTaskCount = 5;

    [ObservableProperty]
    private int completedThisWeek = 12;

    [ObservableProperty]
    private int categoryCount = 4;

    [ObservableProperty]
    private string highPrioritySummary = "2 high priority";

    [ObservableProperty]
    private string categorySummary = "Work, Study, Health...";

    [ObservableProperty]
    private ObservableCollection<FocusItem> focusItems = new();

    public DashboardViewModel()
    {
        Title = "Dashboard";

        FocusItems = new ObservableCollection<FocusItem>
        {
            new("Finish mobile app UI implementation", "High priority task for the dashboard release", "Today", "UI / High", Colors.Blue),
            new("Review task navigation and custom controls", "Check interaction polish across the main flows", "Afternoon", "Review / Med", Colors.Green),
            new("Prepare polished screens for demo", "Tighten spacing, contrast, and layout consistency", "Later", "Demo / Low", Colors.Orange)
        };
    }

    [RelayCommand]
    private Task NewTaskAsync() => NavigateAsync("taskedit");

    [RelayCommand]
    private Task ViewTasksAsync() => NavigateAsync("tasks");

    [RelayCommand]
    private Task ViewCategoriesAsync() => NavigateAsync("categories");
}

public sealed record FocusItem(
    string Title,
    string Subtitle,
    string TimeLabel,
    string MetaLabel,
    Color AccentColor);