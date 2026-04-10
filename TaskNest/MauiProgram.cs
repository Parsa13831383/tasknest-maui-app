using Microsoft.Extensions.Logging;
using TaskNest.Interfaces;
using TaskNest.Services;
using TaskNest.ViewModels;
using TaskNest.Data;

namespace TaskNest;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// 🔥 EXISTING
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddTransient<BaseViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();

		// Database service used during app startup and by data features.
		builder.Services.AddSingleton<AppDatabase>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}