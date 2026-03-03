namespace TaskNest.Views;

[QueryProperty(nameof(TaskId), "id")]
public partial class TaskDetailPage : ContentPage
{
    public string TaskId { get; set; } = "";

    public TaskDetailPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Demo display (you can wire real data later)
        TitleLabel.Text = $"Task {TaskId}";
        DescLabel.Text = "This is the task detail screen (demo).";
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("taskedit");
    }
}