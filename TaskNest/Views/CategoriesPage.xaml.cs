using Microsoft.Extensions.DependencyInjection;
using TaskNest.Interfaces;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class CategoriesPage : ContentPage
{
    public CategoriesPage()
    {
        InitializeComponent();

        var unitOfWork = Application.Current?.Handler?.MauiContext?.Services.GetService<IUnitOfWork>()
            ?? throw new InvalidOperationException("IUnitOfWork service is not registered.");

        BindingContext = new CategoriesViewModel(unitOfWork);
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