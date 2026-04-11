using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly ISupabaseAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(ISupabaseAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) return;

        try
        {
            IsBusy = true;
            await _authService.SignInAsync(Email.Trim(), Password);
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
}