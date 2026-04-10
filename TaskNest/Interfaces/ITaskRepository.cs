using TaskNest.Models;

namespace TaskNest.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskNest.Models.TaskItem>> GetAllAsync();
    Task<TaskNest.Models.TaskItem?> GetByIdAsync(int id);
    Task<int> AddAsync(TaskNest.Models.TaskItem task);
    Task<int> UpdateAsync(TaskNest.Models.TaskItem task);
    Task<int> DeleteAsync(TaskNest.Models.TaskItem task);
}