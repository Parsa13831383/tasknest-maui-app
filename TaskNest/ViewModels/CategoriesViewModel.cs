using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using TaskNest.Interfaces;
using TaskNest.Models;
using CategoryItemModel = TaskNest.Models.CategoryItem;

namespace TaskNest.ViewModels;

public class CategoriesViewModel : BaseViewModel
{
    private static readonly Color[] AccentPalette =
    {
        Color.FromArgb("#3B82F6"), // Electric blue
        Color.FromArgb("#84CC16"), // Sage green
        Color.FromArgb("#F59E0B"), // Deep amber
        Color.FromArgb("#14B8A6"),
        Color.FromArgb("#8B5CF6"),
        Color.FromArgb("#EF4444")
    };

    private readonly IUnitOfWork _unitOfWork;

    public ObservableCollection<CategoryItemModel> Categories { get; } = new();

    private int _totalCategories;
    public int TotalCategories
    {
        get => _totalCategories;
        set
        {
            if (_totalCategories != value)
            {
                _totalCategories = value;
                OnPropertyChanged();
            }
        }
    }

    private string _mostActiveCategory = "N/A";
    public string MostActiveCategory
    {
        get => _mostActiveCategory;
        set
        {
            if (_mostActiveCategory != value)
            {
                _mostActiveCategory = value;
                OnPropertyChanged();
            }
        }
    }

    private int _tasksAssigned;
    public int TasksAssigned
    {
        get => _tasksAssigned;
        set
        {
            if (_tasksAssigned != value)
            {
                _tasksAssigned = value;
                OnPropertyChanged();
            }
        }
    }

    private string _newCategoryName = string.Empty;
    public string NewCategoryName
    {
        get => _newCategoryName;
        set
        {
            if (_newCategoryName != value)
            {
                _newCategoryName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _newCategoryDescription = string.Empty;
    public string NewCategoryDescription
    {
        get => _newCategoryDescription;
        set
        {
            if (_newCategoryDescription != value)
            {
                _newCategoryDescription = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand LoadCategoriesCommand { get; }
    public ICommand AddCategoryCommand { get; }
    public ICommand EditCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ManageCategoriesCommand { get; }
    public ICommand OpenCategoryCommand { get; }

    public CategoriesViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
        AddCategoryCommand = new Command(async () => await AddCategoryAsync());
        EditCategoryCommand = new Command<CategoryItemModel>(async (category) => await EditCategoryAsync(category));
        DeleteCategoryCommand = new Command<CategoryItemModel>(async (category) => await DeleteCategoryAsync(category));
        ManageCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
        OpenCategoryCommand = new Command<CategoryItemModel>(async category => await OpenCategoryAsync(category));
    }

    public async Task LoadCategoriesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await RefreshCategoriesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task AddCategoryAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var generatedName = $"New Category {Categories.Count + 1}";
            var categoryName = string.IsNullOrWhiteSpace(NewCategoryName)
                ? generatedName
                : NewCategoryName.Trim();

            var category = new CategoryItemModel
            {
                Name = categoryName,
                Description = NewCategoryDescription.Trim(),
                Count = 0,
                TaskCountText = "0 tasks",
                BadgeBackgroundColor = Colors.LightGray,
                BadgeTextColor = Colors.Black
            };

            var existingCategories = await _unitOfWork.Categories.GetAllAsync();
            if (existingCategories.Any(c => string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                await Shell.Current.DisplayAlert("Validation", "Category name already exists.", "OK");
                return;
            }

            await _unitOfWork.Categories.AddAsync(category);

            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;

            OnPropertyChanged(nameof(NewCategoryName));
            OnPropertyChanged(nameof(NewCategoryDescription));

            await RefreshCategoriesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task DeleteCategoryAsync(CategoryItemModel? category)
    {
        if (IsBusy) return;
        if (category is null) return;

        try
        {
            IsBusy = true;

            var shouldDelete = await Shell.Current.DisplayAlert(
                "Delete Category",
                $"Delete '{category.Name}'? Tasks in this category will be uncategorized.",
                "Delete",
                "Cancel");

            if (!shouldDelete)
            {
                return;
            }

            await _unitOfWork.Tasks.ClearCategoryAsync(category.Id);
            await _unitOfWork.Categories.DeleteAsync(category);
            await RefreshCategoriesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task UpdateCategoryAsync(CategoryItemModel category)
    {
        if (IsBusy) return;
        if (category is null) return;

        try
        {
            IsBusy = true;

            await _unitOfWork.Categories.UpdateAsync(category);
            await RefreshCategoriesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task EditCategoryAsync(CategoryItemModel? category)
    {
        if (IsBusy) return;
        if (category is null) return;

        var newName = await Shell.Current.DisplayPromptAsync(
            "Edit Category",
            "Update category name",
            "Save",
            "Cancel",
            initialValue: category.Name,
            maxLength: 100);

        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        var normalizedName = newName.Trim();
        if (!string.Equals(normalizedName, category.Name, StringComparison.OrdinalIgnoreCase)
            && Categories.Any(c => c.Id != category.Id && string.Equals(c.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            await Shell.Current.DisplayAlert("Validation", "Category name already exists.", "OK");
            return;
        }

        category.Name = normalizedName;
        await UpdateCategoryAsync(category);
    }

    public async Task OpenCategoryAsync(CategoryItemModel? category)
    {
        if (category is null)
        {
            return;
        }

        var encodedCategory = Uri.EscapeDataString(category.Name ?? string.Empty);
        await Shell.Current.GoToAsync($"//tasks?category={encodedCategory}");
    }

    private async Task RefreshCategoriesAsync()
    {
        Categories.Clear();

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var taskItems = await _unitOfWork.Tasks.GetAllAsync();
        var taskCountByCategoryId = taskItems
            .Where(t => t.CategoryId.HasValue)
            .GroupBy(t => t.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var completedTaskCountByCategoryId = taskItems
            .Where(t => t.CategoryId.HasValue && t.IsCompleted)
            .GroupBy(t => t.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var category in categories)
        {
            var taskCount = taskCountByCategoryId.TryGetValue(category.Id, out var count) ? count : 0;
            var completedCount = completedTaskCountByCategoryId.TryGetValue(category.Id, out var completed) ? completed : 0;

            category.Count = taskCount;
            category.TaskCountText = $"{category.Count} tasks";
            category.CompletedCount = completedCount;
            category.ProgressValue = taskCount == 0 ? 0 : (double)completedCount / taskCount;
            category.ProgressText = $"{completedCount}/{taskCount} tasks completed";
            category.IsEmptyState = taskCount == 0;
            category.AccentColor = GetAccentColor(category.Name);
            category.IconGlyph = GetCategoryGlyph(category.Name);
            category.ShowQuickActions = false;
            category.BadgeBackgroundColor = category.IsEmptyState
                ? Colors.LightGray
                : Color.FromRgba(category.AccentColor.Red, category.AccentColor.Green, category.AccentColor.Blue, 0.14f);
            category.BadgeTextColor = category.IsEmptyState ? Colors.Black : category.AccentColor;

            Categories.Add(category);
        }

        TotalCategories = Categories.Count;
        TasksAssigned = taskItems.Count;
        MostActiveCategory = Categories
            .OrderByDescending(c => c.Count)
            .Select(c => c.Name)
            .FirstOrDefault() ?? "N/A";
    }

    private static Color GetAccentColor(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return AccentPalette[0];
        }

        var hash = Math.Abs(categoryName.GetHashCode());
        return AccentPalette[hash % AccentPalette.Length];
    }

    private static string GetCategoryGlyph(string? categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return "◉";
        }

        var name = categoryName.Trim().ToLowerInvariant();

        if (name.Contains("work")) return "◈";
        if (name.Contains("study") || name.Contains("school")) return "◬";
        if (name.Contains("personal") || name.Contains("home")) return "◎";
        if (name.Contains("health") || name.Contains("gym")) return "◍";
        if (name.Contains("urgent")) return "◆";

        return "◉";
    }
}