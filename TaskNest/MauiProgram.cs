using Microsoft.Extensions.Logging;
using TaskNest.Interfaces;
using TaskNest.Services;
using TaskNest.ViewModels;
using TaskNest.Data;
using TaskNest.Repositories;
using TaskNest.Services.Security;
using TaskNest.Services.Supabase;
using TaskNest.Services.Validation;

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
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddSingleton<ISecureSessionService, SecureSessionService>();
		builder.Services.AddSingleton<IInputValidationService, InputValidationService>();
		builder.Services.AddSingleton<ISupabaseAuthService, SupabaseAuthService>();
		builder.Services.AddTransient<BaseViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<TaskListViewModel>();
		builder.Services.AddTransient<TaskDetailViewModel>();
		builder.Services.AddTransient<TaskEditViewModel>();
		builder.Services.AddTransient<CategoriesViewModel>();

		// Cloud data path (Supabase) used by repositories and view models.
		builder.Services.AddSingleton<ITaskRepository, SupabaseTaskRepository>();
		builder.Services.AddSingleton<ICategoryRepository, SupabaseCategoryRepository>();
		builder.Services.AddSingleton<IUnitOfWork, SupabaseUnitOfWork>();
		builder.Services.AddSingleton<ITaskCloudService, TaskCloudService>();
		builder.Services.AddSingleton<ICategoryCloudService, CategoryCloudService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}