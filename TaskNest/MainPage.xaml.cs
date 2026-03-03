using Microsoft.Maui.Controls;
using TaskNest.ViewModels;

namespace TaskNest;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
	}
}
