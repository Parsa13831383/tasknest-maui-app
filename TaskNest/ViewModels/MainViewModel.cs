using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

    [ObservableProperty]
    private string newTaskText = string.Empty;

    public MainViewModel()
    {
        Title = "Tasks";
    }

    [RelayCommand]
    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskText))
            return;

        Tasks.Insert(0, new TaskItem { Title = NewTaskText.Trim() });
        NewTaskText = string.Empty;
    }

    [RelayCommand]
    private void RemoveTask(TaskItem task)
    {
        if (task is null)
            return;

        if (Tasks.Contains(task))
            Tasks.Remove(task);
    }
}
