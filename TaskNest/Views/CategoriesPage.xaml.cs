using TaskNest.ViewModels;

namespace TaskNest.Views;

public partial class CategoriesPage : ContentPage
{
    public CategoriesPage()
    {
        InitializeComponent();
        BindingContext = new CategoriesViewModel();
    }
}