using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class CompletedTasksPage : ContentPage
{
    public CompletedTasksPage()
    {
        InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<CompletedTasksViewModel>()
            ?? throw new InvalidOperationException("CompletedTasksViewModel service is not registered.");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CompletedTasksViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
