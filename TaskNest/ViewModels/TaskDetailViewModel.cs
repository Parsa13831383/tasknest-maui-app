using System.Windows.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class TaskDetailViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    
    private int _taskId;
    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _reflection = string.Empty;
    private string _dueDate = string.Empty;
    private string _category = string.Empty;
    private bool _isCompleted;

    public int TaskId
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
        set => SetProperty(ref _isCompleted, value);
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
        // Set the Page Title (Inherited from BaseViewModel)
        Title = "Task Details";

        EditCommand = new Command(async () => await GoToEdit());
        DeleteCommand = new Command(async () => await DeleteAsync());
        BackCommand = new Command(async () => await GoBack());
    }

    /// <summary>
    /// Loads task details from the database by task ID
    /// </summary>
    public async Task LoadAsync(int taskId)
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
                Description = task.Description;
                Reflection = string.IsNullOrWhiteSpace(task.Reflection) ? "No reflection yet." : task.Reflection;
                IsCompleted = task.IsCompleted;
                DueDate = task.DueDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                
                // Load category name if CategoryId exists
                if (task.CategoryId.HasValue)
                {
                    var allCategories = await _unitOfWork.Categories.GetAllAsync();
                    var categoryItem = allCategories.FirstOrDefault(c => c.Id == task.CategoryId);
                    Category = categoryItem?.Name ?? string.Empty;
                }
                else
                {
                    Category = string.Empty;
                }
            }
            else
            {
                TaskTitle = string.Empty;
                Description = string.Empty;
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

    private async Task GoToEdit()
    {
        // Navigates to the Edit page with task ID
        await Shell.Current.GoToAsync($"taskedit?id={TaskId}");
    }

    private async Task GoBack()
    {
        // Special ".." syntax tells Shell to go back to the previous page
        await Shell.Current.GoToAsync("..");
    }

    private async Task DeleteAsync()
    {
        if (TaskId <= 0)
        {
            return;
        }

        var shouldDelete = await Shell.Current.DisplayAlert(
            "Delete Task",
            $"Delete '{TaskTitle}'? This is a soft delete.",
            "Delete",
            "Cancel");

        if (!shouldDelete)
        {
            return;
        }

        var task = await _unitOfWork.Tasks.GetByIdAsync(TaskId);
        if (task is null)
        {
            return;
        }

        await _unitOfWork.Tasks.SoftDeleteAsync(task);
        await GoBack();
    }

}