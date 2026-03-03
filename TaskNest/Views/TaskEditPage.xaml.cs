namespace TaskNest.Views;

public partial class TaskEditPage : ContentPage
{
    public TaskEditPage()
    {
        InitializeComponent();
        BindingContext = new TaskEditPageViewModel();
    }
}

public class TaskEditPageViewModel
{
    public List<string> PriorityOptions { get; } = new() { "Low", "Medium", "High" };
}
