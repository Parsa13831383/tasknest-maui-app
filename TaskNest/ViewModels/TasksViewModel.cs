using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using TaskNest.Interfaces;
using TaskNest.Models;
using TaskListItem = TaskNest.ViewModels.TaskListItem;

namespace TaskNest.ViewModels;

public class TaskListViewModel : BaseViewModel
{
    private const string AllCategoriesFilter = "All Categories";

    private readonly IUnitOfWork _unitOfWork;
    private readonly List<TaskListItem> _allTasks = new();
    private string _searchQuery = string.Empty;
    private string _selectedCategoryFilter = AllCategoriesFilter;

    public ObservableCollection<TaskListItem> Tasks { get; } = new();
    public ObservableCollection<string> CategoryFilters { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                ApplyFilters();
            }
        }
    }

    public string SelectedCategoryFilter
    {
        get => _selectedCategoryFilter;
        set
        {
            var normalizedValue = string.IsNullOrWhiteSpace(value) ? AllCategoriesFilter : value;
            if (SetProperty(ref _selectedCategoryFilter, normalizedValue))
            {
                OnPropertyChanged(nameof(FilterButtonText));
                ApplyFilters();
            }
        }
    }

    public string FilterButtonText => SelectedCategoryFilter == AllCategoriesFilter
        ? "Filter Tasks"
        : $"Filter: {SelectedCategoryFilter}";

    public ICommand CreateTaskCommand { get; }
    public ICommand FilterTasksCommand { get; }
    public ICommand ViewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand CompleteTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }

    public TaskListViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Tasks";

        CategoryFilters.Add(AllCategoriesFilter);

        CreateTaskCommand = new Command(async () => await GoToCreate());
        FilterTasksCommand = new Command(async () => await ChooseCategoryFilterAsync());
        ViewTaskCommand = new Command<TaskListItem>(async (task) => await GoToDetails(task));
        EditTaskCommand = new Command<TaskListItem>(async (task) => await GoToEdit(task));
        CompleteTaskCommand = new Command<TaskListItem>(async (task) => await CompleteTaskAsync(task));
        DeleteTaskCommand = new Command<TaskListItem>(async (task) => await DeleteTaskAsync(task));
    }

    public async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            _allTasks.Clear();

            var dbTasks = await _unitOfWork.Tasks.GetAllAsync();
            var dbCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoryNamesById = dbCategories.ToDictionary(c => c.Id, c => c.Name);

            foreach (TaskItem dbTask in dbTasks)
            {
                var categoryText = "Uncategorized";
                if (!string.IsNullOrWhiteSpace(dbTask.CategoryId) && categoryNamesById.TryGetValue(dbTask.CategoryId, out var categoryName))
                {
                    categoryText = categoryName;
                }

                _allTasks.Add(new TaskListItem
                {
                    Id = dbTask.Id,
                    Title = dbTask.Title,
                    Description = dbTask.Description,
                    DueDate = dbTask.DueDate?.ToString("dd MMM yyyy") ?? "No due date",
                    Category = categoryText,
                    TaskColor = dbTask.TaskColor,
                    IsCompleted = dbTask.IsCompleted
                });
            }

            PopulateCategoryFilters();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToCreate()
    {
        await Shell.Current.GoToAsync("taskedit");
    }

    private async Task GoToDetails(TaskListItem? task)
    {
        if (task is null) return;
        await Shell.Current.GoToAsync($"taskdetail?id={task.Id}");
    }

    private async Task GoToEdit(TaskListItem? task)
    {
        if (task is null) return;
        await Shell.Current.GoToAsync($"taskedit?id={task.Id}");
    }

    private async Task DeleteTaskAsync(TaskListItem? task)
    {
        if (IsBusy || task is null) return;

        var shouldDelete = await Shell.Current.DisplayAlert(
            "Delete Task",
            $"Delete '{task.Title}'? This will move it to trash.",
            "Delete",
            "Cancel");

        if (!shouldDelete) return;

        var deleteSucceeded = false;

        try
        {
            IsBusy = true;

            var existing = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
            if (existing is null)
            {
                await Shell.Current.DisplayAlert("Delete Error", "Task not found.", "OK");
                return;
            }

            var rows = await _unitOfWork.Tasks.DeleteAsync(existing);
            if (rows <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Delete Error",
                    "Could not delete task. Please check your connection and try again.",
                    "OK");
                return;
            }

            deleteSucceeded = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during delete: {ex.Message}");
            await Shell.Current.DisplayAlert("Delete Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

        if (deleteSucceeded)
        {
            WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
            await LoadTasksAsync();
        }
    }

    private async Task CompleteTaskAsync(TaskListItem? task)
    {
        if (IsBusy || task is null)
        {
            return;
        }

        if (task.IsCompleted)
        {
            await Shell.Current.DisplayAlert("Already Completed", $"'{task.Title}' is already completed.", "OK");
            return;
        }

        var shouldComplete = await Shell.Current.DisplayAlert(
            "Complete Task",
            $"Mark '{task.Title}' as completed?",
            "Complete",
            "Cancel");

        if (!shouldComplete)
        {
            return;
        }

        // Track whether the cloud update actually succeeded so we only reload on success.
        var updateSucceeded = false;

        try
        {
            IsBusy = true;

            var existing = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
            if (existing is null)
            {
                await Shell.Current.DisplayAlert("Task Not Found", "The selected task could not be loaded.", "OK");
                return;
            }

            if (existing.IsCompleted)
            {
                // Already completed in the cloud — just sync the local flag.
                updateSucceeded = true;
                return;
            }

            existing.IsCompleted = true;
            existing.UpdatedAtUtc = DateTime.UtcNow;

            var updatedRows = await _unitOfWork.Tasks.UpdateAsync(existing);
            if (updatedRows <= 0)
            {
                await Shell.Current.DisplayAlert(
                    "Completion Error",
                    "Could not mark task as completed. Please check your connection and try again.",
                    "OK");
                return;
            }

            updateSucceeded = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error completing task: {ex.Message}");
            await Shell.Current.DisplayAlert("Completion Error", ex.Message, "OK");
        }
        finally
        {
            // Release the lock BEFORE reloading so LoadTasksAsync can proceed.
            IsBusy = false;
        }

        if (updateSucceeded)
        {
            // Notify the Dashboard (and any other subscriber) to reload its completed count.
            WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
            // Refresh the task list so the button shows "Completed".
            await LoadTasksAsync();
        }
    }

    private void PopulateCategoryFilters()
    {
        var categories = _allTasks
            .Select(t => t.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        CategoryFilters.Clear();
        CategoryFilters.Add(AllCategoriesFilter);
        foreach (var category in categories)
        {
            CategoryFilters.Add(category);
        }

        if (!CategoryFilters.Contains(SelectedCategoryFilter))
        {
            SelectedCategoryFilter = AllCategoriesFilter;
        }
    }

    private async Task ChooseCategoryFilterAsync()
    {
        if (Shell.Current is null)
        {
            return;
        }

        var options = CategoryFilters.ToArray();
        var selection = await Shell.Current.DisplayActionSheet(
            "Filter By Category",
            "Cancel",
            null,
            options);

        if (!string.IsNullOrWhiteSpace(selection) && !string.Equals(selection, "Cancel", StringComparison.OrdinalIgnoreCase))
        {
            SelectedCategoryFilter = selection;
        }
    }

    private void ApplyFilters()
    {
        var query = SearchQuery?.Trim();

        IEnumerable<TaskListItem> filteredTasks = _allTasks;

        if (!string.IsNullOrWhiteSpace(query))
        {
            filteredTasks = filteredTasks.Where(t =>
                t.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.Equals(SelectedCategoryFilter, AllCategoriesFilter, StringComparison.OrdinalIgnoreCase))
        {
            filteredTasks = filteredTasks.Where(t =>
                string.Equals(t.Category, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        Tasks.Clear();
        foreach (var task in filteredTasks)
        {
            Tasks.Add(task);
        }
    }
}