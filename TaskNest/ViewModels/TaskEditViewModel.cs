using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class TaskEditViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Dictionary<string, int> _categoryIdByName = new(StringComparer.OrdinalIgnoreCase);
    private int? _editingTaskId;

    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _selectedCategory = string.Empty;
    private string _selectedPriority = "Medium";
    private DateTime _dueDate = DateTime.Today.AddDays(3);
    private bool _isCompleted;
    private bool _isEditMode;

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

    public string SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string SelectedPriority
    {
        get => _selectedPriority;
        set => SetProperty(ref _selectedPriority, value);
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Priorities { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public TaskEditViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        Title = "Create Task";

        Priorities.Add("High");
        Priorities.Add("Medium");
        Priorities.Add("Low");

        SaveCommand = new Command(async () => await SaveTaskAsync());
        CancelCommand = new Command(async () => await GoBackAsync());
    }

    public async Task LoadAsync(int? taskId = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            Categories.Clear();
            _categoryIdByName.Clear();

            var categoryItems = await _unitOfWork.Categories.GetAllAsync();
            foreach (var category in categoryItems)
            {
                Categories.Add(category.Name);
                _categoryIdByName[category.Name] = category.Id;
            }

            if (Categories.Count > 0 && string.IsNullOrWhiteSpace(SelectedCategory))
            {
                SelectedCategory = Categories[0];
            }

            if (taskId.HasValue && taskId.Value > 0)
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId.Value);
                if (task is not null)
                {
                    _editingTaskId = task.Id;
                    IsEditMode = true;
                    Title = "Edit Task";

                    TaskTitle = task.Title;
                    Description = task.Description;
                    SelectedPriority = task.Priority;
                    DueDate = task.DueDate ?? DateTime.Today;
                    IsCompleted = task.IsCompleted;
                    SelectedCategory = task.CategoryId.HasValue
                        ? categoryItems.FirstOrDefault(c => c.Id == task.CategoryId.Value)?.Name ?? string.Empty
                        : string.Empty;
                    return;
                }
            }

            _editingTaskId = null;
            IsEditMode = false;
            Title = "Create Task";
            TaskTitle = string.Empty;
            Description = string.Empty;
            SelectedPriority = "Medium";
            DueDate = DateTime.Today.AddDays(3);
            IsCompleted = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskTitle))
        {
            await Shell.Current.DisplayAlert("Validation", "Task title is required.", "OK");
            return;
        }

        var categoryId = _categoryIdByName.TryGetValue(SelectedCategory ?? string.Empty, out var resolvedCategoryId)
            ? resolvedCategoryId
            : (int?)null;

        if (_editingTaskId.HasValue)
        {
            var existing = await _unitOfWork.Tasks.GetByIdAsync(_editingTaskId.Value);
            if (existing is null)
            {
                await Shell.Current.DisplayAlert("Error", "Task could not be found.", "OK");
                return;
            }

            existing.Title = TaskTitle.Trim();
            existing.Description = Description?.Trim() ?? string.Empty;
            existing.DueDate = DueDate;
            existing.Priority = string.IsNullOrWhiteSpace(SelectedPriority) ? "Medium" : SelectedPriority;
            existing.IsCompleted = IsCompleted;
            existing.CategoryId = categoryId;

            await _unitOfWork.Tasks.UpdateAsync(existing);
            await Shell.Current.DisplayAlert("Saved", "Task updated.", "OK");
        }
        else
        {
            var task = new global::TaskNest.Models.TaskItem
            {
                Title = TaskTitle.Trim(),
                Description = Description?.Trim() ?? string.Empty,
                DueDate = DueDate,
                Priority = string.IsNullOrWhiteSpace(SelectedPriority) ? "Medium" : SelectedPriority,
                IsCompleted = IsCompleted,
                CategoryId = categoryId
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await Shell.Current.DisplayAlert("Saved", "Task created.", "OK");
        }

        await GoBackAsync();
    }

    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}