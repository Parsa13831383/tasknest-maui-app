namespace TaskNest.Views;

public partial class TaskDetailPage : ContentPage
{
    public TaskDetailPage()
    {
        InitializeComponent();
    }

    private async void OnEditTaskClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("taskedit");
    }
}