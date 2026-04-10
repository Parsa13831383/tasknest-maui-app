using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class TaskListViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    public ObservableCollection<TaskListItem> Tasks { get; } = new();

    public ICommand CreateTaskCommand { get; }
    public ICommand ViewTaskCommand { get; }
    public ICommand EditTaskCommand { get; }

    public TaskListViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        Title = "Tasks";

        CreateTaskCommand = new Command(async () => await GoToCreate());
        ViewTaskCommand = new Command<TaskListItem>(async (task) => await GoToDetails(task));
        EditTaskCommand = new Command<TaskListItem>(async (task) => await GoToEdit(task));
    }

    public async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Tasks.Clear();

            var dbTasks = await _unitOfWork.Tasks.GetAllAsync();
            var dbCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoryNamesById = dbCategories.ToDictionary(c => c.Id, c => c.Name);

            foreach (global::TaskNest.Models.TaskItem dbTask in dbTasks)
            {
                var categoryText = "Uncategorized";

                Tasks.Add(new TaskListItem
                {
                    Title = dbTask.Title,
                    Description = dbTask.Description,
                    DueDate = dbTask.DueDate?.ToString("dd MMM yyyy") ?? "No due date",
                    Category = categoryText,
                    PriorityText = dbTask.Priority,
                    PriorityColor = dbTask.PriorityColor
                });
            }
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
        if (task is null)
        {
            return;
        }

        var route = $"taskdetail?" +
                    $"title={Uri.EscapeDataString(task.Title)}&" +
                    $"description={Uri.EscapeDataString(task.Description)}&" +
                    $"dueDate={Uri.EscapeDataString(task.DueDate)}&" +
                    $"category={Uri.EscapeDataString(task.Category)}&" +
                    $"priorityText={Uri.EscapeDataString(task.PriorityText)}";

        await Shell.Current.GoToAsync(route);
    }

    private async Task GoToEdit(TaskListItem? task)
    {
        if (task is null)
        {
            return;
        }

        await Shell.Current.GoToAsync("taskedit");
    }
}