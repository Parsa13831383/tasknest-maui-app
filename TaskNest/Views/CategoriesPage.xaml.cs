using Microsoft.Extensions.DependencyInjection;
using TaskNest.Models;
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

    private void OnCategoryCardPointerEntered(object? sender, PointerEventArgs e)
    {
        var border = ResolveCategoryCardBorder(sender);
        if (border?.BindingContext is CategoryItem category)
        {
            category.ShowQuickActions = true;
            _ = border.ScaleTo(1.02, 160, Easing.CubicOut);
            _ = border.TranslateTo(0, -2, 160, Easing.CubicOut);
        }
    }

    private void OnCategoryCardPointerExited(object? sender, PointerEventArgs e)
    {
        var border = ResolveCategoryCardBorder(sender);
        if (border?.BindingContext is CategoryItem category)
        {
            category.ShowQuickActions = false;
            _ = border.ScaleTo(1.0, 140, Easing.CubicIn);
            _ = border.TranslateTo(0, 0, 140, Easing.CubicIn);
        }
    }

    private async void OnCategoryCardTapped(object? sender, TappedEventArgs e)
    {
        var border = ResolveCategoryCardBorder(sender);
        var category = border?.BindingContext as CategoryItem;

        if (border is not null)
        {
            await border.ScaleTo(0.985, 70, Easing.CubicIn);
            await border.ScaleTo(1.0, 110, Easing.CubicOut);
        }

        if (category is not null && BindingContext is CategoriesViewModel viewModel)
        {
            await viewModel.OpenCategoryAsync(category);
        }
    }

    private static Border? ResolveCategoryCardBorder(object? sender)
    {
        if (sender is Border border)
        {
            return border;
        }

        if (sender is Element element)
        {
            if (element is GestureRecognizer gesture && gesture.Parent is Border gestureParentBorder)
            {
                return gestureParentBorder;
            }

            if (element.Parent is Border parentBorder)
            {
                return parentBorder;
            }
        }

        return null;
    }
}