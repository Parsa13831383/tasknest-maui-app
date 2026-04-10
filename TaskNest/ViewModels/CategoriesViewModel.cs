using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using TaskNest.Interfaces;
using TaskNest.Models;
using CategoryItemModel = TaskNest.Models.CategoryItem;

namespace TaskNest.ViewModels;

public class CategoriesViewModel : BaseViewModel
{
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

    public CategoriesViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
        AddCategoryCommand = new Command(async () => await AddCategoryAsync());
        EditCategoryCommand = new Command<CategoryItemModel>(async (category) => await EditCategoryAsync(category));
        DeleteCategoryCommand = new Command<CategoryItemModel>(async (category) => await DeleteCategoryAsync(category));
        ManageCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
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

    private async Task RefreshCategoriesAsync()
    {
        Categories.Clear();

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var taskItems = await _unitOfWork.Tasks.GetAllAsync();
        var taskCountByCategoryId = taskItems
            .Where(t => t.CategoryId.HasValue)
            .GroupBy(t => t.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var category in categories)
        {
            var taskCount = taskCountByCategoryId.TryGetValue(category.Id, out var count) ? count : 0;
            category.Count = taskCount;
            category.TaskCountText = $"{category.Count} tasks";
            category.BadgeBackgroundColor = Colors.LightGray;
            category.BadgeTextColor = Colors.Black;

            Categories.Add(category);
        }

        TotalCategories = Categories.Count;
        TasksAssigned = taskItems.Count;
        MostActiveCategory = Categories
            .OrderByDescending(c => c.Count)
            .Select(c => c.Name)
            .FirstOrDefault() ?? "N/A";
    }
}