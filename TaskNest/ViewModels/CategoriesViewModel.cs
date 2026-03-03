using System.Collections.ObjectModel;
using TaskNest.Models;

namespace TaskNest.ViewModels;

public class CategoriesViewModel
{
    public ObservableCollection<CategoryItem> Categories { get; } = new();

    public CategoriesViewModel()
    {
        Categories.Add(new CategoryItem { Name = "University", Count = 4 });
        Categories.Add(new CategoryItem { Name = "Work", Count = 2 });
        Categories.Add(new CategoryItem { Name = "Personal", Count = 6 });
    }
}