using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskNest.Interfaces;
using TaskNest.Models;
using TaskListItem = TaskNest.ViewModels.TaskListItem;

namespace TaskNest.ViewModels;

public class TaskListViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    public ObservableCollection<TaskListItem> Tasks { get; } = new();

    public ICommand CreateTaskCommand { get; }
    public ICommand ViewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }

    public TaskListViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Tasks";

        CreateTaskCommand = new Command(async () => await GoToCreate());
        ViewTaskCommand = new Command<TaskListItem>(async (task) => await GoToDetails(task));
        EditTaskCommand = new Command<TaskListItem>(async (task) => await GoToEdit(task));
        DeleteTaskCommand = new Command<TaskListItem>(async (task) => await DeleteTaskAsync(task));
    }

    public async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Safety Refresh: Clear existing UI items before reloading from DB
            Tasks.Clear();

            var dbTasks = await _unitOfWork.Tasks.GetAllAsync();
            var dbCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoryNamesById = dbCategories.ToDictionary(c => c.Id, c => c.Name);

            foreach (TaskItem dbTask in dbTasks)
            {
                var categoryText = "Uncategorized";
                if (dbTask.CategoryId.HasValue && categoryNamesById.TryGetValue(dbTask.CategoryId.Value, out var categoryName))
                {
                    categoryText = categoryName;
                }

                Tasks.Add(new TaskListItem
                {
                    Id = dbTask.Id,
                    Title = dbTask.Title,
                    Description = dbTask.Description,
                    DueDate = dbTask.DueDate?.ToString("dd MMM yyyy") ?? "No due date",
                    Category = categoryText,
                    PriorityText = dbTask.Priority,
                    PriorityColor = dbTask.PriorityColor
                });
            }
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

        try
        {
            var shouldDelete = await Shell.Current.DisplayAlert(
                "Delete Task",
                $"Delete '{task.Title}'? This will move it to trash.",
                "Delete",
                "Cancel");

            if (!shouldDelete) return;

            IsBusy = true;

            // 1. Fetch the actual TaskItem from the database
            var existing = await _unitOfWork.Tasks.GetByIdAsync(task.Id);

            if (existing != null)
            {
                // 2. Perform Soft Delete (Fix 1)
                var rows = await _unitOfWork.Tasks.DeleteAsync(existing);

                // 3. Debug confirmation (Fix 2)
                System.Diagnostics.Debug.WriteLine($"Deleted rows: {rows}");

                if (rows > 0)
                {
                    // 4. Update UI instantly by removing from ObservableCollection
                    Tasks.Remove(task);

                    // 5. Final safety refresh (Fix 3)
                    await LoadTasksAsync();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during delete: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}