using TaskNest.Data;

namespace TaskNest;

public partial class App : Application
{
	private readonly AppDatabase _database;

	public App(AppDatabase database)
	{
		InitializeComponent();

		_database = database;

		Task.Run(async () => await _database.GetConnectionAsync());
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}