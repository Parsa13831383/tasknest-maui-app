using TaskNest.Interfaces;
using TaskNest.Services;

namespace TaskNest;

public partial class App : Application
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly AppShell _appShell;
	private readonly LocalizationService _localization = LocalizationService.Instance;

	public App(IUnitOfWork unitOfWork, AppShell appShell)
	{
		InitializeComponent();

		_unitOfWork = unitOfWork;
		_appShell = appShell;

		// Apply persisted theme preference immediately on startup.
		var isDarkMode = Preferences.Default.Get("settings.darkmode", false);
		UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

		// Ensure localization service is initialized early.
		_ = _localization.CurrentLanguageCode;

		Task.Run(async () => await _unitOfWork.InitializeAsync());
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(_appShell);
	}
}