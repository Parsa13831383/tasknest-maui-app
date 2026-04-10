using Microsoft.Extensions.DependencyInjection;
using TaskNest.Interfaces;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class TaskEditPage : ContentPage
{
    public TaskEditPage()
    {
        InitializeComponent();

        var unitOfWork = Application.Current?.Handler?.MauiContext?.Services.GetService<IUnitOfWork>()
            ?? throw new InvalidOperationException("IUnitOfWork service is not registered.");

        BindingContext = new TaskEditViewModel(unitOfWork);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TaskEditViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}