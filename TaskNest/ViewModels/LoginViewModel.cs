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

    private string _emailError = string.Empty;
    private string _passwordError = string.Empty;
    private string _authError = string.Empty;
    private string _authInfo = string.Empty;

    public string EmailError
    {
        get => _emailError;
        set
        {
            if (SetProperty(ref _emailError, value))
            {
                OnPropertyChanged(nameof(HasEmailError));
            }
        }
    }

    public string PasswordError
    {
        get => _passwordError;
        set
        {
            if (SetProperty(ref _passwordError, value))
            {
                OnPropertyChanged(nameof(HasPasswordError));
            }
        }
    }

    public string AuthError
    {
        get => _authError;
        set
        {
            if (SetProperty(ref _authError, value))
            {
                OnPropertyChanged(nameof(HasAuthError));
            }
        }
    }

    public string AuthInfo
    {
        get => _authInfo;
        set
        {
            if (SetProperty(ref _authInfo, value))
            {
                OnPropertyChanged(nameof(HasAuthInfo));
            }
        }
    }

    public bool HasEmailError => !string.IsNullOrWhiteSpace(EmailError);
    public bool HasPasswordError => !string.IsNullOrWhiteSpace(PasswordError);
    public bool HasAuthError => !string.IsNullOrWhiteSpace(AuthError);
    public bool HasAuthInfo => !string.IsNullOrWhiteSpace(AuthInfo);

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

        EmailError = string.Empty;
        PasswordError = string.Empty;
        AuthError = string.Empty;
        AuthInfo = string.Empty;

        if (!validation.TryValidateEmail(Email, out var normalizedEmail, out var emailError))
        {
            EmailError = emailError;
            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Login", emailError, "OK");
            }
            return;
        }

        if (!validation.TryValidateRequiredText(Password, "Password", out _, out var passwordError, maxLength: 256))
        {
            PasswordError = passwordError;
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
            AuthError = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        if (IsBusy) return;

        EmailError = string.Empty;
        AuthError = string.Empty;
        AuthInfo = string.Empty;

        var emailToUse = Email;

        if (!validation.TryValidateEmail(emailToUse, out var normalizedEmail, out _))
        {
            if (Shell.Current is null)
            {
                EmailError = "Email is required.";
                return;
            }

            var enteredEmail = await Shell.Current.DisplayPromptAsync(
                "Forgot Password",
                "Enter the email address for your TaskNest account.",
                accept: "Send",
                cancel: "Cancel",
                placeholder: "Email",
                maxLength: 320,
                keyboard: Keyboard.Email,
                initialValue: Email);

            if (string.IsNullOrWhiteSpace(enteredEmail))
            {
                return;
            }

            if (!validation.TryValidateEmail(enteredEmail, out normalizedEmail, out var emailError))
            {
                EmailError = emailError;

                if (Shell.Current is not null)
                {
                    await Shell.Current.DisplayAlert("Forgot Password", emailError, "OK");
                }

                return;
            }

            Email = normalizedEmail;
        }

        try
        {
            IsBusy = true;

            await authService.SendPasswordResetEmailAsync(normalizedEmail);
            AuthInfo = "If this email is registered, a password reset link has been sent.";
            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Forgot Password", AuthInfo, "OK");
            }
        }
        catch (Exception ex)
        {
            AuthError = ex.Message;
            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Forgot Password", AuthError, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnEmailChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(EmailError))
        {
            EmailError = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(AuthError))
        {
            AuthError = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(AuthInfo))
        {
            AuthInfo = string.Empty;
        }
    }

    partial void OnPasswordChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(PasswordError))
        {
            PasswordError = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(AuthError))
        {
            AuthError = string.Empty;
        }
    }

    [RelayCommand]
    private Task NavigateToRegisterAsync() => NavigateAsync("register");
}