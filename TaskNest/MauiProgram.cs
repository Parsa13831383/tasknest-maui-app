using Microsoft.Extensions.Logging;
using TaskNest.Interfaces;
using TaskNest.Services;

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

		builder.Services.AddSingleton<INavigationService, NavigationService>();

#if DEBUG
		builder.Logging.AddDebug();

#endif

		return builder.Build();
	}
}
