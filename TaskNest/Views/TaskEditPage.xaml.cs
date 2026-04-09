using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TaskEditPage : ContentPage
{
    public TaskEditPage()
    {
        InitializeComponent();
        BindingContext = new TaskEditViewModel();
    }
}