using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<ProfileViewModel>()
            ?? throw new InvalidOperationException("ProfileViewModel service is not registered.");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel viewModel)
        {
            await viewModel.LoadProfileAsync();
        }
    }
}