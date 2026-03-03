using TaskNest.Views;

namespace TaskNest;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("taskdetail", typeof(TaskDetailPage));
		Routing.RegisterRoute("taskedit", typeof(TaskEditPage));

		Routing.RegisterRoute("login", typeof(LoginPage));
		Routing.RegisterRoute("register", typeof(RegisterPage));
	}
}