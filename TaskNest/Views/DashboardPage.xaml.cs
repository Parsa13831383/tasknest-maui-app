using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<DashboardViewModel>()
            ?? throw new InvalidOperationException("DashboardViewModel service is not registered.");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DashboardViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is DashboardViewModel viewModel)
        {
            viewModel.CancelPendingLoad();
        }
    }
}