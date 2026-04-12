using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<LoginViewModel>()
            ?? throw new InvalidOperationException("LoginViewModel service is not registered.");
    }
}