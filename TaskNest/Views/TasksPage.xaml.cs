using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TasksPage : ContentPage, IQueryAttributable
{
    private string? _requestedCategoryFilter;

    public TasksPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskListViewModel>()
            ?? throw new InvalidOperationException("TaskListViewModel service is not registered.");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _requestedCategoryFilter = null;

        if (!query.TryGetValue("category", out var rawCategory) || rawCategory is null)
        {
            return;
        }

        _requestedCategoryFilter = Uri.UnescapeDataString(rawCategory.ToString() ?? string.Empty);

        if (BindingContext is TaskListViewModel viewModel && !string.IsNullOrWhiteSpace(_requestedCategoryFilter))
        {
            viewModel.SelectedCategoryFilter = _requestedCategoryFilter;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TaskListViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(_requestedCategoryFilter))
            {
                viewModel.SelectedCategoryFilter = _requestedCategoryFilter;
            }

            await viewModel.LoadTasksAsync();
        }
    }
}