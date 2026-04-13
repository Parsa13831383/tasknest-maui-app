using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskNest.Interfaces;
using TaskNest.Services;

namespace TaskNest.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private const string DarkModePreferenceKey = "settings.darkmode";
    private const string ThemePreferenceKey = "settings.theme";
    private const string LanguagePreferenceKey = "settings.language";
    private const string ReminderPreferenceKey = "settings.reminder";

    private readonly LocalizationService _localization = LocalizationService.Instance;
    private readonly ISupabaseAuthService _authService;

    private bool _notificationsEnabled;
    private bool _darkModeEnabled;
    private string _selectedTheme = string.Empty;
    private string _selectedLanguage = string.Empty;
    private string _selectedReminderFrequency = string.Empty;
    private string _securityUserId = "N/A";
    private string _securityAuthState = "Unauthenticated";
    private string _securityTokenState = "No";

    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set => SetProperty(ref _notificationsEnabled, value);
    }

    public bool DarkModeEnabled
    {
        get => _darkModeEnabled;
        set
        {
            if (SetProperty(ref _darkModeEnabled, value))
            {
                ApplyDarkMode(value);
            }
        }
    }

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (SetProperty(ref _selectedTheme, value))
            {
                DarkModeEnabled = string.Equals(value, "Dark", StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value))
            {
                _localization.SetLanguage(value);
            }
        }
    }

    public string SelectedReminderFrequency
    {
        get => _selectedReminderFrequency;
        set => SetProperty(ref _selectedReminderFrequency, value);
    }

    public ObservableCollection<string> Themes { get; } = new();
    public ObservableCollection<string> Languages { get; } = new();
    public ObservableCollection<string> ReminderFrequencies { get; } = new();

    public string SecurityUserId
    {
        get => _securityUserId;
        set => SetProperty(ref _securityUserId, value);
    }

    public string SecurityAuthState
    {
        get => _securityAuthState;
        set => SetProperty(ref _securityAuthState, value);
    }

    public string SecurityTokenState
    {
        get => _securityTokenState;
        set => SetProperty(ref _securityTokenState, value);
    }

    public ICommand SaveSettingsCommand { get; }
    public ICommand ResetSettingsCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand RefreshSecurityProofCommand { get; }

    public SettingsViewModel(ISupabaseAuthService authService)
    {
        _authService = authService;
        Title = "Settings";

        Themes.Add("Light");
        Themes.Add("Dark");
        Themes.Add("System Default");

        Languages.Add("English");
        Languages.Add("French");
        Languages.Add("Spanish");

        ReminderFrequencies.Add("Daily");
        ReminderFrequencies.Add("Weekly");
        ReminderFrequencies.Add("Only for deadlines");

        NotificationsEnabled = true;
        LoadPreferences();

        SaveSettingsCommand = new Command(async () => await SaveSettings());
        ResetSettingsCommand = new Command(ResetSettings);
        LogoutCommand = new Command(async () => await Logout());
        RefreshSecurityProofCommand = new Command(RefreshSecurityProof);
        RefreshSecurityProof();
    }

    private async Task SaveSettings()
    {
        Preferences.Default.Set(DarkModePreferenceKey, DarkModeEnabled);
        Preferences.Default.Set(ThemePreferenceKey, SelectedTheme);
        Preferences.Default.Set(LanguagePreferenceKey, SelectedLanguage);
        Preferences.Default.Set(ReminderPreferenceKey, SelectedReminderFrequency);

        await Shell.Current.DisplayAlert(
            _localization.Translate("Settings"),
            _localization.Translate("Settings saved successfully."),
            _localization.Translate("OK"));
    }

    private void ResetSettings()
    {
        NotificationsEnabled = true;
        DarkModeEnabled = false;
        SelectedTheme = "Light";
        SelectedLanguage = "English";
        SelectedReminderFrequency = "Daily";

        Preferences.Default.Set(DarkModePreferenceKey, DarkModeEnabled);
        Preferences.Default.Set(ThemePreferenceKey, SelectedTheme);
        Preferences.Default.Set(LanguagePreferenceKey, SelectedLanguage);
        Preferences.Default.Set(ReminderPreferenceKey, SelectedReminderFrequency);
    }

    private async Task Logout()
    {
        await _authService.SignOutAsync();
        RefreshSecurityProof();

        await Shell.Current.DisplayAlert(
            _localization.Translate("Logout"),
            _localization.Translate("You have been signed out."),
            _localization.Translate("OK"));

        await Shell.Current.GoToAsync("login");
    }

    public void RefreshSecurityProof()
    {
        SecurityUserId = string.IsNullOrWhiteSpace(_authService.UserId)
            ? "N/A"
            : _authService.UserId!;
        SecurityAuthState = _authService.IsAuthenticated ? "Authenticated" : "Unauthenticated";
        SecurityTokenState = string.IsNullOrWhiteSpace(_authService.AccessToken) ? "No" : "Yes";
    }

    private void LoadPreferences()
    {
        var savedDarkMode = Preferences.Default.Get(DarkModePreferenceKey, false);
        var savedTheme = Preferences.Default.Get(ThemePreferenceKey, savedDarkMode ? "Dark" : "Light");
        var savedLanguage = Preferences.Default.Get(LanguagePreferenceKey, "English");
        var savedReminder = Preferences.Default.Get(ReminderPreferenceKey, "Daily");

        _darkModeEnabled = savedDarkMode;
        OnPropertyChanged(nameof(DarkModeEnabled));
        ApplyDarkMode(savedDarkMode);

        _selectedTheme = savedTheme;
        OnPropertyChanged(nameof(SelectedTheme));

        _selectedLanguage = savedLanguage;
        OnPropertyChanged(nameof(SelectedLanguage));
        _localization.SetLanguage(savedLanguage);

        _selectedReminderFrequency = savedReminder;
        OnPropertyChanged(nameof(SelectedReminderFrequency));
    }

    private static void ApplyDarkMode(bool isDarkMode)
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        Preferences.Default.Set(DarkModePreferenceKey, isDarkMode);
    }
}