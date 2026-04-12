using TaskNest.Interfaces;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        var authService = Application.Current?.Handler?.MauiContext?.Services.GetService<ISupabaseAuthService>()
            ?? throw new InvalidOperationException("ISupabaseAuthService is not registered in DI.");

        BindingContext = new LoginViewModel(authService);
    }
}