using TaskNest.Models;

namespace TaskNest.Interfaces;

public interface ITaskRepository
{
    Task<List<global::TaskNest.Models.TaskItem>> GetAllAsync();
    Task<global::TaskNest.Models.TaskItem?> GetByIdAsync(int id);
    Task<int> AddAsync(global::TaskNest.Models.TaskItem task);
    Task<int> UpdateAsync(global::TaskNest.Models.TaskItem task);
    Task<int> DeleteAsync(global::TaskNest.Models.TaskItem task);
}