using System.Windows.Input;

namespace TaskNest.ViewModels;

public class TaskDetailViewModel : BaseViewModel
{
    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _dueDate = string.Empty;
    private string _category = string.Empty;
    private string _priorityText = string.Empty;

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

    public string PriorityText
    {
        get => _priorityText;
        set => SetProperty(ref _priorityText, value);
    }

    public ICommand EditCommand { get; }
    public ICommand BackCommand { get; }

    public TaskDetailViewModel()
    {
        // Set the Page Title (Inherited from BaseViewModel)
        Title = "Task Details";

        EditCommand = new Command(async () => await GoToEdit());
        BackCommand = new Command(async () => await GoBack());
    }

    private async Task GoToEdit()
    {
        // Navigates to the Edit page
        await Shell.Current.GoToAsync("taskedit");
    }

    private async Task GoBack()
    {
        // Special ".." syntax tells Shell to go back to the previous page
        await Shell.Current.GoToAsync("..");
    }
}