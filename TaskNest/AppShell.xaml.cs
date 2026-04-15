using TaskNest.Views;
using TaskNest.Services;
using TaskNest.Interfaces;
using System.Net;
using TaskNest.ViewModels;

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
		Routing.RegisterRoute("completedtasks", typeof(CompletedTasksPage));
		Routing.RegisterRoute("login", typeof(LoginPage));
		Routing.RegisterRoute("register", typeof(RegisterPage));
		Routing.RegisterRoute("resetpassword", typeof(ResetPasswordPage));

		ApplyLocalizedShellText();
		_localization.LanguageChanged += OnLanguageChanged;
		Navigated += OnShellNavigated;
		Navigating += OnShellNavigating;
		Loaded += OnShellLoaded;
		_authService.SessionExpired += OnSessionExpired;
	}

	private void OnShellLoaded(object? sender, EventArgs e)
	{
		_ = ApplyInitialRouteAndPendingDeepLinkAsync();

		if (CurrentPage is Page page)
		{
			_localization.ApplyToPage(page);
		}
	}

	private async Task ApplyInitialRouteAndPendingDeepLinkAsync()
	{
		await ApplyInitialRouteAsync();

		if (Application.Current is App app)
		{
			await app.FlushPendingDeepLinkAsync();
		}
	}

	private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
	{
		var route = e.Current?.Location.OriginalString ?? string.Empty;
		var isAuthRoute = route.Contains("login", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("register", StringComparison.OrdinalIgnoreCase)
			|| route.Contains("resetpassword", StringComparison.OrdinalIgnoreCase);

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
		var navigatingToResetPassword = targetRoute.Contains("resetpassword", StringComparison.OrdinalIgnoreCase);
		var navigatingToProtected = IsProtectedRoute(targetRoute);

		if (navigatingToProtected && !_authService.IsAuthenticated)
		{
			e.Cancel();
			_ = RedirectToLoginAsync();
			return;
		}

		if (navigatingToAuth && _authService.IsAuthenticated && !navigatingToResetPassword)
		{
			e.Cancel();
			_ = RedirectToDashboardAsync();
		}
	}

	public async Task HandleIncomingUrlAsync(Uri uri)
	{
		if (!IsPasswordRecoveryUri(uri))
		{
			return;
		}

		if (!TryGetRecoveryAccessToken(uri, out var accessToken))
		{
			await MainThread.InvokeOnMainThreadAsync(() =>
				Shell.Current?.DisplayAlert("Reset Password", "The reset link is missing a token.", "OK"));
			return;
		}

		await _authService.ApplyRecoverySessionAsync(accessToken);
		await GoToAsync("resetpassword");
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

	private static bool IsPasswordRecoveryUri(Uri uri)
	{
		return uri.Scheme.Equals("tasknest", StringComparison.OrdinalIgnoreCase)
			&& (uri.Host.Equals("reset-password", StringComparison.OrdinalIgnoreCase)
				|| uri.AbsolutePath.Contains("reset-password", StringComparison.OrdinalIgnoreCase)
				|| uri.AbsolutePath.Contains("resetpassword", StringComparison.OrdinalIgnoreCase));
	}

	private static bool TryGetRecoveryAccessToken(Uri uri, out string accessToken)
	{
		var parameters = ParseParameters(uri);

		if (parameters.TryGetValue("access_token", out accessToken!) && !string.IsNullOrWhiteSpace(accessToken))
		{
			return true;
		}

		accessToken = string.Empty;
		return false;
	}

	private static Dictionary<string, string> ParseParameters(Uri uri)
	{
		var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		ParseParameterString(uri.Query, parameters);
		ParseParameterString(uri.Fragment, parameters);
		return parameters;
	}

	private static void ParseParameterString(string value, IDictionary<string, string> parameters)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return;
		}

		var trimmed = value.TrimStart('?', '#');

		foreach (var pair in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries))
		{
			var parts = pair.Split('=', 2);
			var key = WebUtility.UrlDecode(parts[0]);
			if (string.IsNullOrWhiteSpace(key))
			{
				continue;
			}

			var decodedValue = parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty;
			parameters[key] = decodedValue ?? string.Empty;
		}
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