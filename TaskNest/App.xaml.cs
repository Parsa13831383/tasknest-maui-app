using TaskNest.Interfaces;

namespace TaskNest;

public partial class App : Application
{
	private readonly IUnitOfWork _unitOfWork;

	public App(IUnitOfWork unitOfWork)
	{
		InitializeComponent();

		_unitOfWork = unitOfWork;

		Task.Run(async () => await _unitOfWork.InitializeAsync());
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}