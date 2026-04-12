using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TaskDetailPage : ContentPage, IQueryAttributable
{
    private int taskId;

    public TaskDetailPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskDetailViewModel>()
            ?? throw new InvalidOperationException("TaskDetailViewModel service is not registered.");
    }

    public int TaskId
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

        if (idValue is int intId)
        {
            TaskId = intId;
            return;
        }

        if (int.TryParse(idValue.ToString(), out var parsedId))
        {
            TaskId = parsedId;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadTaskDetailsAsync();
    }

    private async Task LoadTaskDetailsAsync()
    {
        if (TaskId <= 0 || BindingContext is not TaskDetailViewModel viewModel)
            return;

        await viewModel.LoadAsync(TaskId);
    }
}
