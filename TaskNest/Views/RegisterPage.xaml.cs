using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = new RegisterViewModel();
    }
}