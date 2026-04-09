using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class CategoriesViewModel : BaseViewModel
{
    private int _totalCategories;
    private string _mostActiveCategory = string.Empty;
    private int _tasksAssigned;

    public int TotalCategories
    {
        get => _totalCategories;
        set => SetProperty(ref _totalCategories, value);
    }

    public string MostActiveCategory
    {
        get => _mostActiveCategory;
        set => SetProperty(ref _mostActiveCategory, value);
    }

    public int TasksAssigned
    {
        get => _tasksAssigned;
        set => SetProperty(ref _tasksAssigned, value);
    }

    public ObservableCollection<CategoryItem> Categories { get; } = new();

    public ICommand AddCategoryCommand { get; }
    public ICommand ManageCategoriesCommand { get; }

    public CategoriesViewModel()
    {
        Title = "Categories";

        TotalCategories = 4;
        MostActiveCategory = "University";
        TasksAssigned = 12;

        Categories.Add(new CategoryItem
        {
            Name = "University",
            Description = "Assignments, coursework, and revision tasks.",
            TaskCountText = "4 Tasks",
            BadgeBackgroundColor = Color.FromArgb("#DBEAFE"),
            BadgeTextColor = Color.FromArgb("#1D4ED8")
        });

        Categories.Add(new CategoryItem
        {
            Name = "Work",
            Description = "Shifts, responsibilities, and job-related tasks.",
            TaskCountText = "2 Tasks",
            BadgeBackgroundColor = Color.FromArgb("#DCFCE7"),
            BadgeTextColor = Color.FromArgb("#15803D")
        });

        Categories.Add(new CategoryItem
        {
            Name = "Personal",
            Description = "Gym, lifestyle, errands, and self-development goals.",
            TaskCountText = "6 Tasks",
            BadgeBackgroundColor = Color.FromArgb("#FEF3C7"),
            BadgeTextColor = Color.FromArgb("#B45309")
        });

        Categories.Add(new CategoryItem
        {
            Name = "Health",
            Description = "Meal prep, recovery, running, and physical wellbeing.",
            TaskCountText = "3 Tasks",
            BadgeBackgroundColor = Color.FromArgb("#FCE7F3"),
            BadgeTextColor = Color.FromArgb("#BE185D")
        });

        AddCategoryCommand = new Command(OnAddCategory);
        ManageCategoriesCommand = new Command(OnManageCategories);
    }

    private async void OnAddCategory()
    {
        await Shell.Current.DisplayAlert("Add Category", "Add Category action triggered.", "OK");
    }

    private async void OnManageCategories()
    {
        await Shell.Current.DisplayAlert("Manage Categories", "Manage Categories action triggered.", "OK");
    }
}