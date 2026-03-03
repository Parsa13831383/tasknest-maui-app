using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TasksPage : ContentPage
{
    public TasksPage()
    {
        InitializeComponent();
        BindingContext = new TasksViewModel();
    }
}