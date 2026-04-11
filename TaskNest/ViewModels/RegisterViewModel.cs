using System.Windows.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService _authService;

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public ICommand RegisterCommand { get; }

    public RegisterViewModel(ISupabaseAuthService authService)
    {
        _authService = authService;
        RegisterCommand = new Command(async () => await RegisterAsync());
    }

    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;

        try
        {
            IsBusy = true;
            await _authService.SignUpAsync(Email.Trim(), Password);
            await Shell.Current.DisplayAlert("Success", "Account created successfully.", "OK");
            await Shell.Current.GoToAsync("//login");
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
}