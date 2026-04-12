using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TaskDetailPage : ContentPage, IQueryAttributable
{
    private string taskId = string.Empty;

    public TaskDetailPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskDetailViewModel>()
            ?? throw new InvalidOperationException("TaskDetailViewModel service is not registered.");
    }

    public string TaskId
    {
        get => taskId;
        set
        {
            taskId = value;
            _ = LoadTaskDetailsAsync();
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var idValue) || idValue is null)
        {
            return;
        }

        TaskId = idValue.ToString() ?? string.Empty;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadTaskDetailsAsync();
    }

    private async Task LoadTaskDetailsAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskId) || BindingContext is not TaskDetailViewModel viewModel)
            return;

        await viewModel.LoadAsync(TaskId);
    }
}
