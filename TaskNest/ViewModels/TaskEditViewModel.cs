using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TaskNest.ViewModels;

public class TaskEditViewModel : BaseViewModel
{
    private string _taskTitle = string.Empty;
    private string _description = string.Empty;
    private string _selectedCategory = string.Empty;
    private string _selectedPriority = string.Empty;
    private DateTime _dueDate = DateTime.Today.AddDays(3);
    private bool _isCompleted;

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

    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Priorities { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public TaskEditViewModel()
    {
        Title = "Edit Task";

        Categories.Add("Coursework");
        Categories.Add("Development");
        Categories.Add("Documentation");
        Categories.Add("Personal");

        Priorities.Add("High");
        Priorities.Add("Medium");
        Priorities.Add("Low");

        TaskTitle = "Finish UI navigation";
        Description = "Complete the remaining page flow and visual consistency.";
        SelectedCategory = "Coursework";
        SelectedPriority = "High";
        DueDate = DateTime.Today.AddDays(3);
        IsCompleted = false;

        SaveCommand = new Command(async () => await SaveTask());
        CancelCommand = new Command(async () => await GoBack());
    }

    private async Task SaveTask()
    {
        await Shell.Current.DisplayAlert("Saved", "Task changes saved successfully.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}