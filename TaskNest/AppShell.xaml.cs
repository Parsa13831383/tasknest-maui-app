using TaskNest.Views;
using TaskNest.Services;

namespace TaskNest;

public partial class AppShell : Shell
{
	private readonly LocalizationService _localization = LocalizationService.Instance;

	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("taskdetail", typeof(TaskDetailPage));
		Routing.RegisterRoute("taskedit", typeof(TaskEditPage));
		Routing.RegisterRoute("login", typeof(LoginPage));
		Routing.RegisterRoute("register", typeof(RegisterPage));

		ApplyLocalizedShellText();
		_localization.LanguageChanged += OnLanguageChanged;
		Navigated += OnShellNavigated;
		Loaded += OnShellLoaded;
	}

	private void OnShellLoaded(object? sender, EventArgs e)
	{
		if (CurrentPage is Page page)
		{
			_localization.ApplyToPage(page);
		}
	}

	private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		if (CurrentPage is Page page)
		{
			_localization.ApplyToPage(page);
		}
	}

	private void OnLanguageChanged(object? sender, EventArgs e)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			ApplyLocalizedShellText();

			if (CurrentPage is Page page)
			{
				_localization.ApplyToPage(page);
			}
		});
	}

	private void ApplyLocalizedShellText()
	{
		Title = _localization.Translate("TaskNest");

		DashboardFlyout.Title = _localization.Translate("Dashboard");
		DashboardContent.Title = _localization.Translate("Dashboard");

		TasksFlyout.Title = _localization.Translate("Tasks");
		TasksContent.Title = _localization.Translate("Tasks");

		CategoriesFlyout.Title = _localization.Translate("Categories");
		CategoriesContent.Title = _localization.Translate("Categories");

		SettingsFlyout.Title = _localization.Translate("Settings");
		SettingsContent.Title = _localization.Translate("Settings");

		ProfileFlyout.Title = _localization.Translate("Profile");
		ProfileContent.Title = _localization.Translate("Profile");
	}
}