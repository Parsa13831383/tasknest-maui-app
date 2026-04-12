using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService authService;
    private readonly IInputValidationService validation;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe = true;

    public LoginViewModel(ISupabaseAuthService authService, IInputValidationService validation)
    {
        this.authService = authService;
        this.validation = validation;
        Title = "Login";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (!validation.TryValidateEmail(Email, out var normalizedEmail, out var emailError))
        {
            await Shell.Current.DisplayAlert("Validation", emailError, "OK");
            return;
        }

        if (!validation.TryValidateRequiredText(Password, "Password", out _, out var passwordError, maxLength: 256))
        {
            await Shell.Current.DisplayAlert("Validation", passwordError, "OK");
            return;
        }

        try
        {
            IsBusy = true;

            await authService.SignInAsync(normalizedEmail, Password, RememberMe);

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