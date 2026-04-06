namespace TaskNest.Views;

public partial class TasksPage : ContentPage
{
    public TasksPage()
    {
        InitializeComponent();
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("taskedit");
    }
}