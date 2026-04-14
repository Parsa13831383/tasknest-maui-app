using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class TaskDetailViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    private string _taskId = string.Empty;
    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _reflection = string.Empty;
    private string _dueDate = string.Empty;
    private string _category = string.Empty;
    private bool _isCompleted;
    private string _statusLabel = "Active";

    public string TaskId
    {
        get => _taskId;
        set => SetProperty(ref _taskId, value);
    }

    public string TaskTitle
    {
        get => _taskTitle;
        set => SetProperty(ref _taskTitle, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Reflection
    {
        get => _reflection;
        set => SetProperty(ref _reflection, value);
    }

    public string DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                OnPropertyChanged(nameof(IsNotCompleted));
                StatusLabel = value ? "Completed" : "Active";
            }
        }
    }

    public bool IsNotCompleted => !IsCompleted;

    public string StatusLabel
    {
        get => _statusLabel;
        set => SetProperty(ref _statusLabel, value);
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Task Details";

        EditCommand = new Command(async () => await GoToEdit());
        DeleteCommand = new Command(async () => await DeleteAsync());
        CompleteCommand = new Command(async () => await CompleteAsync());
        BackCommand = new Command(async () => await GoBack());
    }

    public async Task LoadAsync(string taskId)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            TaskId = taskId;

            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);

            if (task != null)
            {
                TaskTitle = task.Title;
                Description = string.IsNullOrWhiteSpace(task.Description) ? "No description." : task.Description;
                Reflection = string.IsNullOrWhiteSpace(task.Reflection) ? "No reflection yet." : task.Reflection;
                IsCompleted = task.IsCompleted;
                DueDate = task.DueDate?.ToString("ddd, dd MMM yyyy") ?? "No due date";

                if (!string.IsNullOrWhiteSpace(task.CategoryId))
                {
                    var allCategories = await _unitOfWork.Categories.GetAllAsync();
                    var categoryItem = allCategories.FirstOrDefault(c => c.Id == task.CategoryId);
                    Category = categoryItem?.Name ?? "Uncategorized";
                }
                else
                {
                    Category = "Uncategorized";
                }
            }
            else
            {
                TaskTitle = string.Empty;
                Description = "No description.";
                Reflection = "No reflection yet.";
                IsCompleted = false;
                DueDate = string.Empty;
                Category = string.Empty;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CompleteAsync()
    {
        if (IsBusy || IsCompleted || string.IsNullOrWhiteSpace(TaskId))
        {
            return;
        }

        var shouldComplete = await Shell.Current.DisplayAlert(
            "Complete Task",
            $"Mark '{TaskTitle}' as completed?",
            "Complete",
            "Cancel");

        if (!shouldComplete) return;

        try
        {
            IsBusy = true;

            var task = await _unitOfWork.Tasks.GetByIdAsync(TaskId);
            if (task is null) return;

            task.IsCompleted = true;
            task.UpdatedAtUtc = DateTime.UtcNow;

            var rows = await _unitOfWork.Tasks.UpdateAsync(task);
            if (rows > 0)
            {
                IsCompleted = true;
                WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Could not mark task as completed.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToEdit()
    {
        await Shell.Current.GoToAsync($"taskedit?id={TaskId}");
    }

    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskId)) return;

        var shouldDelete = await Shell.Current.DisplayAlert(
            "Delete Task",
            $"Delete '{TaskTitle}'? This is a soft delete.",
            "Delete",
            "Cancel");

        if (!shouldDelete) return;

        try
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(TaskId);
            if (task is null) return;

            await _unitOfWork.Tasks.SoftDeleteAsync(task);
            WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
            await GoBack();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}