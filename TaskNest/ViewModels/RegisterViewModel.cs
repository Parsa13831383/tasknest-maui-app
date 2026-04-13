using System.Windows.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService authService;
    private readonly IInputValidationService validation;

    private string name = string.Empty;
    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    private string email = string.Empty;
    public string Email
    {
        get => email;
        set => SetProperty(ref email, value);
    }

    private string password = string.Empty;
    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    private string confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => confirmPassword;
        set => SetProperty(ref confirmPassword, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand NavigateToLoginCommand { get; }

    public RegisterViewModel(ISupabaseAuthService authService, IInputValidationService validation)
    {
        this.authService = authService;
        this.validation = validation;
        Title = "Register";
        RegisterCommand = new Command(async () => await RegisterAsync());
        NavigateToLoginCommand = new Command(async () => await NavigateToLoginAsync());
    }

    private async Task RegisterAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (!validation.TryValidateEmail(Email, out var normalizedEmail, out var emailError))
        {
            await Shell.Current.DisplayAlert("Validation", emailError, "OK");
            return;
        }

        if (!validation.TryValidatePassword(Password, out var normalizedPassword, out var passwordError))
        {
            await Shell.Current.DisplayAlert("Validation", passwordError, "OK");
            return;
        }

        if (!string.Equals(normalizedPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            await Shell.Current.DisplayAlert("Validation", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var result = await authService.SignUpAsync(normalizedEmail, normalizedPassword, Name);
            var requiresEmailConfirmation = string.IsNullOrWhiteSpace(result?.AccessToken);

            await Shell.Current.DisplayAlert(
                "Success",
                requiresEmailConfirmation
                    ? "Account created. Please confirm your email from your inbox (or spam folder), then log in."
                    : "Account created successfully.",
                "OK");

            await NavigateToLoginAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Registration Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task NavigateToLoginAsync() => NavigateAsync("login");
}