using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using TaskNest.Interfaces;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class CompletedTasksViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    public ObservableCollection<TaskListItem> CompletedTasks { get; } = new();

    public bool HasCompletedTasks => CompletedTasks.Count > 0;

    public ICommand UndoTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }

    public CompletedTasksViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Completed Tasks";

        UndoTaskCommand = new Command<TaskListItem>(async (task) => await UndoTaskAsync(task));
        DeleteTaskCommand = new Command<TaskListItem>(async (task) => await DeleteTaskAsync(task));
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var dbTasks = await _unitOfWork.Tasks.GetAllAsync();
            var dbCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoryNamesById = dbCategories.ToDictionary(c => c.Id, c => c.Name);

            CompletedTasks.Clear();

            foreach (var dbTask in dbTasks.Where(t => !t.IsDeleted && t.IsCompleted))
            {
                var categoryText = "Uncategorized";
                if (!string.IsNullOrWhiteSpace(dbTask.CategoryId) && categoryNamesById.TryGetValue(dbTask.CategoryId, out var name))
                {
                    categoryText = name;
                }

                CompletedTasks.Add(new TaskListItem
                {
                    Id = dbTask.Id,
                    Title = dbTask.Title,
                    Description = dbTask.Description,
                    DueDate = dbTask.DueDate?.ToString("dd MMM yyyy") ?? "No due date",
                    Category = categoryText,
                    TaskColor = dbTask.TaskColor,
                    IsCompleted = true
                });
            }

            OnPropertyChanged(nameof(HasCompletedTasks));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading completed tasks: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UndoTaskAsync(TaskListItem? task)
    {
        if (IsBusy || task is null) return;

        try
        {
            IsBusy = true;

            var existing = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
            if (existing is null)
            {
                await Shell.Current.DisplayAlert("Error", "Task not found.", "OK");
                return;
            }

            existing.IsCompleted = false;
            existing.UpdatedAtUtc = DateTime.UtcNow;

            var rows = await _unitOfWork.Tasks.UpdateAsync(existing);
            if (rows <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Could not restore task. Please try again.", "OK");
                return;
            }

            WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

        await LoadAsync();
    }

    private async Task DeleteTaskAsync(TaskListItem? task)
    {
        if (IsBusy || task is null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Delete Task",
            $"Permanently delete '{task.Title}'?",
            "Delete", "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;

            var existing = await _unitOfWork.Tasks.GetByIdAsync(task.Id);
            if (existing is null)
            {
                await Shell.Current.DisplayAlert("Error", "Task not found.", "OK");
                return;
            }

            var rows = await _unitOfWork.Tasks.DeleteAsync(existing);
            if (rows <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Could not delete task. Please try again.", "OK");
                return;
            }

            WeakReferenceMessenger.Default.Send(new TaskStatusChangedMessage());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }

        await LoadAsync();
    }
}
