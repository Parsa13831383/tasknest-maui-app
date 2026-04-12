using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe = true;

    public LoginViewModel(ISupabaseAuthService authService)
    {
        this.authService = authService;
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;

        try
        {
            IsBusy = true;

            await authService.SignInAsync(Email.Trim(), Password, RememberMe);

            await Shell.Current.GoToAsync("//dashboard");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Login Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task NavigateToRegisterAsync() => NavigateAsync("register");
}