using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TasksPage : ContentPage
{
    public TasksPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskListViewModel>()
            ?? throw new InvalidOperationException("TaskListViewModel service is not registered.");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TaskListViewModel viewModel)
        {
            await viewModel.LoadTasksAsync();
        }
    }
}