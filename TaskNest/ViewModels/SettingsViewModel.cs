using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TaskNest.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private bool _notificationsEnabled;
    private bool _darkModeEnabled;
    private string _selectedTheme = string.Empty;
    private string _selectedLanguage = string.Empty;
    private string _selectedDefaultPriority = string.Empty;
    private string _selectedReminderFrequency = string.Empty;

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set => SetProperty(ref _notificationsEnabled, value);
    }

    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set => SetProperty(ref _darkModeEnabled, value);
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public string SelectedDefaultPriority
    {
        get => _selectedDefaultPriority;
        set => SetProperty(ref _selectedDefaultPriority, value);
    }

    public string SelectedReminderFrequency
    {
        get => _selectedReminderFrequency;
        set => SetProperty(ref _selectedReminderFrequency, value);
    }

    public ObservableCollection<string> Themes { get; } = new();
    public ObservableCollection<string> Languages { get; } = new();
    public ObservableCollection<string> Priorities { get; } = new();
    public ObservableCollection<string> ReminderFrequencies { get; } = new();

    public ICommand SaveSettingsCommand { get; }
    public ICommand ResetSettingsCommand { get; }
    public ICommand LogoutCommand { get; }

    public SettingsViewModel()
    {
        Title = "Settings";

        Themes.Add("Light");
        Themes.Add("Dark");
        Themes.Add("System Default");

        Languages.Add("English");
        Languages.Add("Spanish");
        Languages.Add("Kurdish");

        Priorities.Add("Low");
        Priorities.Add("Medium");
        Priorities.Add("High");

        ReminderFrequencies.Add("Daily");
        ReminderFrequencies.Add("Weekly");
        ReminderFrequencies.Add("Only for deadlines");

        NotificationsEnabled = true;
        DarkModeEnabled = false;
        SelectedTheme = "Light";
        SelectedLanguage = "English";
        SelectedDefaultPriority = "Medium";
        SelectedReminderFrequency = "Daily";

        SaveSettingsCommand = new Command(async () => await SaveSettings());
        ResetSettingsCommand = new Command(ResetSettings);
        LogoutCommand = new Command(async () => await Logout());
    }

    private async Task SaveSettings()
    {
        await Shell.Current.DisplayAlert("Settings", "Settings saved successfully.", "OK");
    }

    private void ResetSettings()
    {
        NotificationsEnabled = true;
        DarkModeEnabled = false;
        SelectedTheme = "Light";
        SelectedLanguage = "English";
        SelectedDefaultPriority = "Medium";
        SelectedReminderFrequency = "Daily";
    }

    private async Task Logout()
    {
        await Shell.Current.DisplayAlert("Logout", "You have been signed out.", "OK");
        await Shell.Current.GoToAsync("login");
    }
}