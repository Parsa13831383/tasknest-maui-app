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

    // -- Inline validation error properties --

    private string nameError = string.Empty;
    public string NameError
    {
        get => nameError;
        set { if (SetProperty(ref nameError, value)) OnPropertyChanged(nameof(HasNameError)); }
    }
    public bool HasNameError => !string.IsNullOrWhiteSpace(NameError);

    private string emailError = string.Empty;
    public string EmailError
    {
        get => emailError;
        set { if (SetProperty(ref emailError, value)) OnPropertyChanged(nameof(HasEmailError)); }
    }
    public bool HasEmailError => !string.IsNullOrWhiteSpace(EmailError);

    private string passwordError = string.Empty;
    public string PasswordError
    {
        get => passwordError;
        set { if (SetProperty(ref passwordError, value)) OnPropertyChanged(nameof(HasPasswordError)); }
    }
    public bool HasPasswordError => !string.IsNullOrWhiteSpace(PasswordError);

    private string confirmPasswordError = string.Empty;
    public string ConfirmPasswordError
    {
        get => confirmPasswordError;
        set { if (SetProperty(ref confirmPasswordError, value)) OnPropertyChanged(nameof(HasConfirmPasswordError)); }
    }
    public bool HasConfirmPasswordError => !string.IsNullOrWhiteSpace(ConfirmPasswordError);

    private string authError = string.Empty;
    public string AuthError
    {
        get => authError;
        set { if (SetProperty(ref authError, value)) OnPropertyChanged(nameof(HasAuthError)); }
    }
    public bool HasAuthError => !string.IsNullOrWhiteSpace(AuthError);

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

    private void ClearErrors()
    {
        NameError = string.Empty;
        EmailError = string.Empty;
        PasswordError = string.Empty;
        ConfirmPasswordError = string.Empty;
        AuthError = string.Empty;
    }

    private async Task RegisterAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ClearErrors();

        var hasError = false;

        if (string.IsNullOrWhiteSpace(Name))
        {
            NameError = "Full name is required.";
            hasError = true;
        }

        if (!validation.TryValidateEmail(Email, out var normalizedEmail, out var emailErr))
        {
            EmailError = emailErr;
            hasError = true;
        }

        if (!validation.TryValidatePassword(Password, out var normalizedPassword, out var passwordErr))
        {
            PasswordError = passwordErr;
            hasError = true;
        }
        else if (!string.Equals(normalizedPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            ConfirmPasswordError = "Passwords do not match.";
            hasError = true;
        }

        if (hasError) return;

        try
        {
            IsBusy = true;

            var result = await authService.SignUpAsync(normalizedEmail!, normalizedPassword!, Name);
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
            AuthError = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task NavigateToLoginAsync() => NavigateAsync("login");
}