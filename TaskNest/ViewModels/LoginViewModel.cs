using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TaskNest.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe = true;

    public LoginViewModel()
    {
        Title = "Login";
    }

    [RelayCommand]
    private void Login()
    {
        // Placeholder for now
        // Later this can call an authentication service
    }

    [RelayCommand]
    private Task NavigateToRegister()
    {
        return NavigateAsync("register");
    }
}