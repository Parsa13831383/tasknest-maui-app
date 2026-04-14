using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class ResetPasswordViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService authService;
    private readonly IInputValidationService validation;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    private string errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            if (SetProperty(ref errorMessage, value))
            {
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }
    }

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ResetPasswordViewModel(ISupabaseAuthService authService, IInputValidationService validation)
    {
        this.authService = authService;
        this.validation = validation;
        Title = "Reset Password";
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (!validation.TryValidatePassword(NewPassword, out var normalizedPassword, out var passwordError))
        {
            ErrorMessage = passwordError;

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Validation", passwordError, "OK");
            }

            return;
        }

        if (!string.Equals(normalizedPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            const string mismatchMessage = "Passwords do not match.";
            ErrorMessage = mismatchMessage;

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Validation", mismatchMessage, "OK");
            }

            return;
        }

        try
        {
            IsBusy = true;

            await authService.UpdatePasswordAsync(normalizedPassword);
            await authService.SignOutAsync();

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Password Updated", "Your password has been reset. Please sign in again.", "OK");
            }

            await NavigateAsync("login");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;

            if (Shell.Current is not null)
            {
                await Shell.Current.DisplayAlert("Reset Password", ex.Message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task NavigateToLoginAsync() => NavigateAsync("login");

    partial void OnNewPasswordChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
            ErrorMessage = string.Empty;
        }
    }

    partial void OnConfirmPasswordChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
            ErrorMessage = string.Empty;
        }
    }
}