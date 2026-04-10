using Microsoft.Extensions.DependencyInjection;
using TaskNest.Interfaces;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TasksPage : ContentPage
{
    public TasksPage()
    {
        InitializeComponent();

        var unitOfWork = Application.Current?.Handler?.MauiContext?.Services.GetService<IUnitOfWork>()
            ?? throw new InvalidOperationException("IUnitOfWork service is not registered.");

        BindingContext = new TaskListViewModel(unitOfWork);
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