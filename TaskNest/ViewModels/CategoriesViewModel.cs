using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using TaskNest.Interfaces;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class CategoriesViewModel : BaseViewModel
{
    private readonly IUnitOfWork _unitOfWork;

    public ObservableCollection<CategoryItem> Categories { get; } = new();

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
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ManageCategoriesCommand { get; }

    public CategoriesViewModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
        AddCategoryCommand = new Command(async () => await AddCategoryAsync());
        DeleteCategoryCommand = new Command<CategoryItem>(async (category) => await DeleteCategoryAsync(category));
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

            var category = new CategoryItem
            {
                Name = categoryName,
                Description = NewCategoryDescription.Trim(),
                Count = 0,
                TaskCountText = "0 tasks",
                BadgeBackgroundColor = Colors.LightGray,
                BadgeTextColor = Colors.Black
            };

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

    public async Task DeleteCategoryAsync(CategoryItem? category)
    {
        if (IsBusy) return;
        if (category is null) return;

        try
        {
            IsBusy = true;

            await _unitOfWork.Categories.DeleteAsync(category);
            await RefreshCategoriesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task UpdateCategoryAsync(CategoryItem category)
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

    private async Task RefreshCategoriesAsync()
    {
        Categories.Clear();

        var categories = await _unitOfWork.Categories.GetAllAsync();

        foreach (var category in categories)
        {
            category.TaskCountText = $"{category.Count} tasks";
            category.BadgeBackgroundColor = Colors.LightGray;
            category.BadgeTextColor = Colors.Black;

            Categories.Add(category);
        }

        TotalCategories = Categories.Count;
        TasksAssigned = Categories.Sum(c => c.Count);
        MostActiveCategory = Categories
            .OrderByDescending(c => c.Count)
            .Select(c => c.Name)
            .FirstOrDefault() ?? "N/A";
    }
}