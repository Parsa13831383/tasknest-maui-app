using TaskNest.Views;
using TaskNest.Services;
using TaskNest.Interfaces;

namespace TaskNest;

public partial class AppShell : Shell
{
	private readonly LocalizationService _localization = LocalizationService.Instance;
	private readonly ISupabaseAuthService _authService;
	private bool _initialNavigationApplied;

	public AppShell(ISupabaseAuthService authService)
	{
		_authService = authService;
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
		_ = ApplyInitialRouteAsync();

		if (CurrentPage is Page page)
		{
			_localization.ApplyToPage(page);
		}
	}

	private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		var route = e.Current?.Location.OriginalString ?? string.Empty;
		var isAuthRoute = route.Contains("login", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("register", StringComparison.OrdinalIgnoreCase);

		FlyoutBehavior = isAuthRoute ? FlyoutBehavior.Disabled : FlyoutBehavior.Flyout;

		if (CurrentPage is Page page)
		{
			_localization.ApplyToPage(page);
		}
	}

	private async Task ApplyInitialRouteAsync()
	{
		if (_initialNavigationApplied)
		{
			return;
		}

		_initialNavigationApplied = true;

		if (_authService.IsAuthenticated || await _authService.RestoreSessionAsync())
		{
			await GoToAsync("//dashboard");
			return;
		}

		await GoToAsync("login");
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