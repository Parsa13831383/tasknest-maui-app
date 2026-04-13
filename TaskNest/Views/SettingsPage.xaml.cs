using TaskNest.Interfaces;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();

        var authService = Application.Current?.Handler?.MauiContext?.Services.GetService<ISupabaseAuthService>()
            ?? throw new InvalidOperationException("ISupabaseAuthService is not registered in DI.");

        _viewModel = new SettingsViewModel(authService);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshSecurityProof();
    }
}