using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TaskEditPage : ContentPage, IQueryAttributable
{
    private int? _taskId;

    public TaskEditPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskEditViewModel>()
            ?? throw new InvalidOperationException("TaskEditViewModel service is not registered.");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _taskId = null;

        if (!query.TryGetValue("id", out var rawId) || rawId is null)
        {
            return;
        }

        if (rawId is int intId)
        {
            _taskId = intId;
            return;
        }

        if (int.TryParse(rawId.ToString(), out var parsedId))
        {
            _taskId = parsedId;
        }

        if (BindingContext is TaskEditViewModel viewModel)
        {
            _ = viewModel.LoadAsync(_taskId);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TaskEditViewModel viewModel)
        {
            await viewModel.LoadAsync(_taskId);
        }
    }
}