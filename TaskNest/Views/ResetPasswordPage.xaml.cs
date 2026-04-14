using Microsoft.Extensions.DependencyInjection;
using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class ResetPasswordPage : ContentPage
{
    public ResetPasswordPage()
    {
        InitializeComponent();

        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<ResetPasswordViewModel>()
            ?? throw new InvalidOperationException("ResetPasswordViewModel service is not registered.");
    }
}