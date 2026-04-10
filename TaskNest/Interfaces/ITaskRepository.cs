using TaskNest.Models;

namespace TaskNest.Interfaces;

public interface ITaskRepository
{
    Task<List<global::TaskNest.Models.TaskItem>> GetAllAsync();
    Task<List<global::TaskNest.Models.TaskItem>> GetByCategoryIdAsync(int categoryId);
    Task<global::TaskNest.Models.TaskItem?> GetByIdAsync(int id);
    Task<int> CountActiveAsync();
    Task<int> CountCompletedAsync();
    Task<int> CountActiveByCategoryAsync(int categoryId);
    Task<int> AddAsync(global::TaskNest.Models.TaskItem task);
    Task<int> UpdateAsync(global::TaskNest.Models.TaskItem task);
    Task<int> ClearCategoryAsync(int categoryId);
    Task<int> SoftDeleteAsync(global::TaskNest.Models.TaskItem task);
    Task<int> DeleteAsync(global::TaskNest.Models.TaskItem task);
}