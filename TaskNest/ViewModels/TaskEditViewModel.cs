using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using TaskNest.Interfaces;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class TaskEditViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Dictionary<string, int> _categoryIdByName = new(StringComparer.OrdinalIgnoreCase);

    private int? _editingTaskId;
    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _reflection = string.Empty;
    private string _selectedCategory = string.Empty;
    private string _selectedTaskColor = "#6B7280";
    private Color _selectedTaskColorPreview = Colors.Gray;
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

    public string Reflection
    {
        get => _reflection;
        set => SetProperty(ref _reflection, value);
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string SelectedTaskColor
    {
        get => _selectedTaskColor;
        set
        {
            if (SetProperty(ref _selectedTaskColor, value))
            {
                SelectedTaskColorPreview = ParseColorOrDefault(value, Colors.Gray);
                UpdateSelectedColorOption(value);
            }
        }
    }

    public Color SelectedTaskColorPreview
    {
        get => _selectedTaskColorPreview;
        set => SetProperty(ref _selectedTaskColorPreview, value);
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

    public ObservableCollection<TaskColorOption> AvailableTaskColors { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectTaskColorCommand { get; }

    public TaskEditViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Create Task";

        InitializeColorOptions();

        SaveCommand = new Command(async () => await SaveTaskAsync());
        CancelCommand = new Command(async () => await GoBackAsync());
        SelectTaskColorCommand = new Command<TaskColorOption>(SelectTaskColor);
    }

    public async Task LoadAsync(int? taskId = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            _categoryIdByName.Clear();

            var categoryItems = await _unitOfWork.Categories.GetAllAsync();
            foreach (var category in categoryItems)
            {
                _categoryIdByName[category.Name] = category.Id;
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
                    Reflection = task.Reflection;
                    SelectedTaskColor = string.IsNullOrWhiteSpace(task.TaskColorHex)
                        ? "#6B7280"
                        : task.TaskColorHex;
                    DueDate = task.DueDate ?? DateTime.Today;
                    IsCompleted = task.IsCompleted;

                    if (task.CategoryId.HasValue)
                    {
                        var cat = categoryItems.FirstOrDefault(c => c.Id == task.CategoryId.Value);
                        SelectedCategory = cat?.Name ?? string.Empty;
                    }

                    return;
                }
            }

            _editingTaskId = null;
            IsEditMode = false;
            Title = "Create Task";
            TaskTitle = string.Empty;
            Description = string.Empty;
            Reflection = string.Empty;
            SelectedCategory = string.Empty;
            SelectedTaskColor = "#6B7280";
            DueDate = DateTime.Today.AddDays(3);
            IsCompleted = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading TaskEdit data: {ex.Message}");
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

        var categoryId = await ResolveCategoryIdAsync(SelectedCategory);
        var taskColorHex = NormalizeColorHex(SelectedTaskColor);

        if (_editingTaskId.HasValue)
        {
            var existing = await _unitOfWork.Tasks.GetByIdAsync(_editingTaskId.Value);
            if (existing is null) return;

            existing.Title = TaskTitle.Trim();
            existing.Description = Description?.Trim() ?? string.Empty;
            existing.Reflection = Reflection?.Trim() ?? string.Empty;
            existing.DueDate = DueDate;
            existing.TaskColorHex = taskColorHex;
            existing.IsCompleted = IsCompleted;
            existing.CategoryId = categoryId;

            await _unitOfWork.Tasks.UpdateAsync(existing);
        }
        else
        {
            var task = new TaskItem
            {
                Title = TaskTitle.Trim(),
                Description = Description?.Trim() ?? string.Empty,
                Reflection = Reflection?.Trim() ?? string.Empty,
                DueDate = DueDate,
                TaskColorHex = taskColorHex,
                IsCompleted = IsCompleted,
                CategoryId = categoryId
            };

            await _unitOfWork.Tasks.AddAsync(task);
        }

        await GoBackAsync();
    }

    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task<int?> ResolveCategoryIdAsync(string? categoryName)
    {
        var normalizedName = categoryName?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return null;
        }

        if (_categoryIdByName.TryGetValue(normalizedName, out var existingCategoryId))
        {
            return existingCategoryId;
        }

        var category = new CategoryItem
        {
            Name = normalizedName,
            Description = string.Empty
        };

        await _unitOfWork.Categories.AddAsync(category);
        _categoryIdByName[normalizedName] = category.Id;
        return category.Id;
    }

    private void InitializeColorOptions()
    {
        AvailableTaskColors.Clear();

        var hexColors = new[]
        {
            "#6B7280",
            "#EF4444",
            "#FB923C",
            "#FACC15",
            "#4ADE80",
            "#38BDF8",
            "#8B5CF6",
            "#9CA3AF"
        };

        foreach (var hex in hexColors)
        {
            AvailableTaskColors.Add(new TaskColorOption(hex));
        }

        UpdateSelectedColorOption(SelectedTaskColor);
    }

    private void SelectTaskColor(TaskColorOption? selectedOption)
    {
        if (selectedOption is null)
        {
            return;
        }

        SelectedTaskColor = selectedOption.Hex;
    }

    private void UpdateSelectedColorOption(string? rawColor)
    {
        var normalized = rawColor?.Trim() ?? string.Empty;

        foreach (var option in AvailableTaskColors)
        {
            option.IsSelected = string.Equals(option.Hex, normalized, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static string NormalizeColorHex(string? rawColor)
    {
        if (!string.IsNullOrWhiteSpace(rawColor) && Color.TryParse(rawColor.Trim(), out _))
        {
            return rawColor.Trim();
        }

        return "#6B7280";
    }

    private static Color ParseColorOrDefault(string? rawColor, Color fallback)
    {
        if (!string.IsNullOrWhiteSpace(rawColor) && Color.TryParse(rawColor.Trim(), out var parsedColor))
        {
            return parsedColor;
        }

        return fallback;
    }

}