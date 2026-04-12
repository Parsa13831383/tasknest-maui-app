using TaskNest.Models;

namespace TaskNest.Interfaces;

public interface ITaskRepository
{
    Task<List<global::TaskNest.Models.TaskItem>> GetAllAsync();
    Task<List<global::TaskNest.Models.TaskItem>> GetByCategoryIdAsync(string categoryId);
    Task<global::TaskNest.Models.TaskItem?> GetByIdAsync(string id);
    Task<int> CountActiveAsync();
    Task<int> CountCompletedAsync();
    Task<int> CountActiveByCategoryAsync(string categoryId);
    Task<int> AddAsync(global::TaskNest.Models.TaskItem task);
    Task<int> UpdateAsync(global::TaskNest.Models.TaskItem task);
    Task<int> ClearCategoryAsync(string categoryId);
    Task<int> SoftDeleteAsync(global::TaskNest.Models.TaskItem task);
    Task<int> DeleteAsync(global::TaskNest.Models.TaskItem task);
}