using TaskNest.Models;

namespace TaskNest.Interfaces;

public interface ICategoryRepository
{
    Task<List<CategoryItem>> GetAllAsync();
    Task<CategoryItem?> GetByIdAsync(string id);
    Task<int> AddAsync(CategoryItem category);
    Task<int> UpdateAsync(CategoryItem category);
    Task<int> DeleteAsync(CategoryItem category);
}