using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using TaskNest.Interfaces;

namespace TaskNest.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IProfileService profileService;

    [ObservableProperty]
    private string fullName = "TaskNest User";

    [ObservableProperty]
    private string emailAddress = "Not available";

    [ObservableProperty]
    private string role = "Guest";

    [ObservableProperty]
    private string sessionStatus = "Signed out";

    [ObservableProperty]
    private string initials = "TU";

    [ObservableProperty]
    private int completedTaskCount;

    [ObservableProperty]
    private int activeTaskCount;

    [ObservableProperty]
    private int categoryCount;

    [ObservableProperty]
    private bool isAuthenticated;

    private string errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            if (SetProperty(ref errorMessage, value))
            {
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }
    }

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsProfileReadOnly => true;

    public ProfileViewModel(IProfileService profileService)
    {
        this.profileService = profileService;
        Title = "Profile";

        // Live-refresh profile stats whenever a task changes anywhere in the app.
        WeakReferenceMessenger.Default.Register<TaskStatusChangedMessage>(this, (_, _) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!IsBusy)
                {
                    await LoadProfileAsync();
                }
            });
        });
    }

    [RelayCommand]
    public async Task LoadProfileAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        try
        {
            IsBusy = true;

            var summary = await profileService.GetProfileSummaryAsync();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                FullName = summary.FullName;
                EmailAddress = summary.Email;
                Role = summary.Role;
                SessionStatus = summary.SessionStatus;
                Initials = summary.Initials;
                IsAuthenticated = summary.IsAuthenticated;
                CompletedTaskCount = summary.CompletedTaskCount;
                ActiveTaskCount = summary.ActiveTaskCount;
                CategoryCount = summary.CategoryCount;
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}