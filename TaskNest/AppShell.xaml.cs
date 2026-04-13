using TaskNest.Views;
using TaskNest.Services;
using TaskNest.Interfaces;

namespace TaskNest;

public partial class AppShell : Shell
{
	private readonly LocalizationService _localization = LocalizationService.Instance;
	private readonly ISupabaseAuthService _authService;
	private bool _initialNavigationApplied;
	private bool _isRedirecting;

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
		Navigating += OnShellNavigating;
		Loaded += OnShellLoaded;
		_authService.SessionExpired += OnSessionExpired;
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

	private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
	{
		if (_isRedirecting)
		{
			return;
		}

		var targetRoute = e.Target.Location.OriginalString;
		var navigatingToAuth = IsAuthRoute(targetRoute);
		var navigatingToProtected = IsProtectedRoute(targetRoute);

		if (navigatingToProtected && !_authService.IsAuthenticated)
		{
			e.Cancel();
			_ = RedirectToLoginAsync();
			return;
		}

		if (navigatingToAuth && _authService.IsAuthenticated)
		{
			e.Cancel();
			_ = RedirectToDashboardAsync();
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

	private async void OnSessionExpired(object? sender, string message)
	{
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await RedirectToLoginAsync(showSessionExpiredMessage: true, message: message);
		});
	}

	private async Task RedirectToLoginAsync(bool showSessionExpiredMessage = false, string message = "Session expired")
	{
		if (_isRedirecting)
		{
			return;
		}

		try
		{
			_isRedirecting = true;
			await GoToAsync("login");

			if (showSessionExpiredMessage)
			{
				await Shell.Current.DisplayAlert("Authentication", message, "OK");
			}
		}
		finally
		{
			_isRedirecting = false;
		}
	}

	private async Task RedirectToDashboardAsync()
	{
		if (_isRedirecting)
		{
			return;
		}

		try
		{
			_isRedirecting = true;
			await GoToAsync("//dashboard");
		}
		finally
		{
			_isRedirecting = false;
		}
	}

	private static bool IsAuthRoute(string route)
	{
		return route.Contains("login", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("register", StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsProtectedRoute(string route)
	{
		return route.Contains("dashboard", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("tasks", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("categories", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("settings", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("profile", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("taskdetail", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("taskedit", StringComparison.OrdinalIgnoreCase);
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