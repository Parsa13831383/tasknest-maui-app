using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

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

    public DashboardViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Dashboard";

        FocusItems = new ObservableCollection<FocusItem>
        {
            new("Finish mobile app UI implementation", "High priority task for the dashboard release", "Today", "UI / High", Colors.Blue),
            new("Review task navigation and custom controls", "Check interaction polish across the main flows", "Afternoon", "Review / Med", Colors.Green),
            new("Prepare polished screens for demo", "Tighten spacing, contrast, and layout consistency", "Later", "Demo / Low", Colors.Orange)
        };
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

            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            var categories = await _unitOfWork.Categories.GetAllAsync();

            TodayTaskCount = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today && !t.IsCompleted);
            CompletedThisWeek = tasks.Count(t => t.IsCompleted && t.UpdatedAtUtc >= DateTime.UtcNow.AddDays(-7));
            CategoryCount = categories.Count;

            var highPriorityOpen = tasks.Count(t => !t.IsCompleted && string.Equals(t.Priority, "High", StringComparison.OrdinalIgnoreCase));
            HighPrioritySummary = $"{highPriorityOpen} high priority";

            var topCategories = categories
                .OrderByDescending(c => tasks.Count(t => t.CategoryId == c.Id))
                .Take(3)
                .Select(c => c.Name)
                .ToList();

            CategorySummary = topCategories.Count > 0
                ? string.Join(", ", topCategories)
                : "No categories yet";
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
}

public sealed record FocusItem(
    string Title,
    string Subtitle,
    string TimeLabel,
    string MetaLabel,
    Color AccentColor);