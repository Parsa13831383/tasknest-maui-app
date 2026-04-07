using System.Windows.Input;

namespace TaskNest.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand NavigateToLoginCommand { get; }

    public RegisterViewModel()
    {
        Title = "Register";

        RegisterCommand = new Command(OnRegister);
        NavigateToLoginCommand = new Command(async () => await GoToLogin());
    }

    private void OnRegister()
    {
        // Later:
        // validation + API call
    }

    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("login");
    }
}