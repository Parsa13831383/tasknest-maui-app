using TaskNest.Models.Cloud;

namespace TaskNest.Interfaces;

public interface ITaskCloudService
{
    Task<List<CloudTaskDto>> GetTasksAsync();
    Task<CloudTaskDto?> GetTaskByIdAsync(string id);
    Task<List<CloudTaskDto>> CreateTaskAsync(CloudTaskDto task);
    Task<List<CloudTaskDto>> UpdateTaskAsync(string id, CloudTaskDto task);
    Task<List<CloudTaskDto>> SoftDeleteTaskAsync(string id);
}