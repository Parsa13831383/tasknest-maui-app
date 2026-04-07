using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TaskNest.ViewModels;

public class TaskListViewModel : BaseViewModel
{
    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public ICommand CreateTaskCommand { get; }
    public ICommand ViewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }

    public TaskListViewModel()
    {
        Title = "Tasks";

        // Sample data (for UI demo)
        Tasks.Add(new TaskItem
        {
            Title = "Finish UI navigation",
            Description = "Complete the remaining page flow and visual consistency.",
            DueDate = "10 Apr 2026",
            Category = "Coursework",
            PriorityText = "High",
            PriorityColor = Colors.Red
        });

        Tasks.Add(new TaskItem
        {
            Title = "Refactor services",
            Description = "Clean up app service layer and prepare for MVVM wiring.",
            DueDate = "11 Apr 2026",
            Category = "Development",
            PriorityText = "Medium",
            PriorityColor = Colors.Orange
        });

        Tasks.Add(new TaskItem
        {
            Title = "Write README",
            Description = "Document setup, features, and app structure clearly.",
            DueDate = "12 Apr 2026",
            Category = "Documentation",
            PriorityText = "Low",
            PriorityColor = Colors.Green
        });

        CreateTaskCommand = new Command(async () => await GoToCreate());
        ViewTaskCommand = new Command<TaskItem>(async (task) => await GoToDetails(task));
        EditTaskCommand = new Command<TaskItem>(async (task) => await GoToEdit(task));
    }

    private async Task GoToCreate()
    {
        await Shell.Current.GoToAsync("taskedit");
    }

    private async Task GoToDetails(TaskItem task)
    {
        var route = $"taskdetail?" +
                    $"title={Uri.EscapeDataString(task.Title)}&" +
                    $"description={Uri.EscapeDataString(task.Description)}&" +
                    $"dueDate={Uri.EscapeDataString(task.DueDate)}&" +
                    $"category={Uri.EscapeDataString(task.Category)}&" +
                    $"priorityText={Uri.EscapeDataString(task.PriorityText)}";

        await Shell.Current.GoToAsync(route);
    }

    private async Task GoToEdit(TaskItem task)
    {
        await Shell.Current.GoToAsync("taskedit");
    }
}