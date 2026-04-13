using TaskNest.Interfaces;
using TaskNest.Models;
using TaskNest.Models.Cloud;
using TaskNest.Models.Enums;

namespace TaskNest.Repositories;

// Implements repository operations for tasks using cloud-backed data services.
public class SupabaseTaskRepository : ITaskRepository
{
    private readonly ITaskCloudService _taskCloudService;
    private readonly ISupabaseAuthService _authService;

    public SupabaseTaskRepository(
        ITaskCloudService taskCloudService,
        ISupabaseAuthService authService)
    {
        _taskCloudService = taskCloudService;
        _authService = authService;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        EnsureAuthenticated();

        var cloudTasks = await _taskCloudService.GetTasksAsync();
        return cloudTasks.Select(MapToTaskItem).OrderBy(t => t.Title).ToList();
    }

    public async Task<List<TaskItem>> GetByCategoryIdAsync(string categoryId)
    {
        var tasks = await GetAllAsync();
        return tasks.Where(t => t.CategoryId == categoryId).ToList();
    }

    public async Task<TaskItem?> GetByIdAsync(string id)
    {
        EnsureAuthenticated();

        var cloudTask = await _taskCloudService.GetTaskByIdAsync(id);
        return cloudTask is null ? null : MapToTaskItem(cloudTask);
    }

    public async Task<int> CountActiveAsync()
    {
        var tasks = await GetAllAsync();
        return tasks.Count(t => !t.IsDeleted);
    }

    public async Task<int> CountCompletedAsync()
    {
        var tasks = await GetAllAsync();
        return tasks.Count(t => !t.IsDeleted && t.IsCompleted);
    }

    public async Task<int> CountActiveByCategoryAsync(string categoryId)
    {
        var tasks = await GetByCategoryIdAsync(categoryId);
        return tasks.Count(t => !t.IsDeleted);
    }

    public async Task<int> AddAsync(TaskItem task)
    {
        EnsureAuthenticated();

        var created = await _taskCloudService.CreateTaskAsync(new CloudTaskDto
        {
            UserId = _authService.UserId ?? string.Empty,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CategoryId = task.CategoryId,
            Priority = "Low"
        });

        var createdItem = created.FirstOrDefault();
        if (createdItem?.Id is null)
        {
            return 0;
        }

        task.Id = createdItem.Id ?? string.Empty;
        task.CreatedAtUtc = createdItem.CreatedAtUtc;
        task.UpdatedAtUtc = createdItem.UpdatedAtUtc;
        task.SyncStatus = SyncStatus.Synced;
        task.IsDeleted = false;
        return 1;
    }

    public async Task<int> UpdateAsync(TaskItem task)
    {
        EnsureAuthenticated();

        if (string.IsNullOrWhiteSpace(task.Id))
        {
            return 0;
        }

        var updated = await _taskCloudService.UpdateTaskAsync(task.Id, new CloudTaskDto
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            CategoryId = task.CategoryId,
            Priority = "Low"
        });

        var updatedItem = updated.FirstOrDefault();
        if (updatedItem is null)
        {
            return 0;
        }

        task.UpdatedAtUtc = updatedItem.UpdatedAtUtc;
        task.SyncStatus = SyncStatus.Synced;
        return 1;
    }

    public async Task<int> ClearCategoryAsync(string categoryId)
    {
        var tasks = await GetByCategoryIdAsync(categoryId);
        var updatedCount = 0;

        foreach (var task in tasks)
        {
            task.CategoryId = null;
            updatedCount += await UpdateAsync(task);
        }

        return updatedCount;
    }

    public async Task<int> SoftDeleteAsync(TaskItem task)
    {
        EnsureAuthenticated();

        if (string.IsNullOrWhiteSpace(task.Id))
        {
            return 0;
        }

        var deleted = await _taskCloudService.SoftDeleteTaskAsync(task.Id);
        if (!deleted.Any())
        {
            return 0;
        }

        task.IsDeleted = true;
        task.UpdatedAtUtc = DateTime.UtcNow;
        task.SyncStatus = SyncStatus.Synced;
        return 1;
    }

    public Task<int> DeleteAsync(TaskItem task) => SoftDeleteAsync(task);

    private TaskItem MapToTaskItem(CloudTaskDto cloudTask)
    {
        return new TaskItem
        {
            Id = cloudTask.Id ?? string.Empty,
            Title = cloudTask.Title,
            Description = cloudTask.Description,
            DueDate = cloudTask.DueDate,
            IsCompleted = cloudTask.IsCompleted,
            CategoryId = cloudTask.CategoryId,
            CreatedAtUtc = cloudTask.CreatedAtUtc,
            UpdatedAtUtc = cloudTask.UpdatedAtUtc,
            IsDeleted = cloudTask.IsDeleted,
            SyncStatus = SyncStatus.Synced
        };
    }

    private void EnsureAuthenticated()
    {
        if (!_authService.IsAuthenticated)
        {
            throw new InvalidOperationException("Please log in before accessing cloud data.");
        }
    }
}