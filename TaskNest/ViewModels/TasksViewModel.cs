using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class TasksViewModel
{
    public ObservableCollection<TaskItemVM> Tasks { get; } = new();

    public TasksViewModel()
    {
        // sample data
        Tasks.Add(new TaskItemVM("Finish UI navigation", "Wire Shell flyout + routes", "High"));
        Tasks.Add(new TaskItemVM("Refactor services", "Clean up NavigationService", "Medium"));
        Tasks.Add(new TaskItemVM("Write README", "Add screenshots + setup steps", "Low"));
    }
}

public class TaskItemVM
{
    public string Title { get; }
    public string Subtitle { get; }
    public string Priority { get; }

    public Color PriorityColor => Priority switch
    {
        "High" => Colors.Red,
        "Medium" => Colors.Orange,
        "Low" => Colors.Green,
        _ => Colors.Gray
    };

    public TaskItemVM(string title, string subtitle, string priority)
    {
        Title = title;
        Subtitle = subtitle;
        Priority = priority;
    }
}