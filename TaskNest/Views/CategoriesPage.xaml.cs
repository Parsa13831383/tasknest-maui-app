using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class CategoriesPage : ContentPage
{
    public CategoriesPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<CategoriesViewModel>()
            ?? throw new InvalidOperationException("CategoriesViewModel service is not registered.");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CategoriesViewModel viewModel)
        {
            await viewModel.LoadCategoriesAsync();
        }
    }
}