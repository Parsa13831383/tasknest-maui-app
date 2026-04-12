using TaskNest.Data;
using TaskNest.Interfaces;
using TaskNest.Models.Enums;

namespace TaskNest.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDatabase _database;

    public TaskRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<global::TaskNest.Models.TaskItem>> GetAllAsync()
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<List<global::TaskNest.Models.TaskItem>> GetByCategoryIdAsync(string categoryId)
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted && t.CategoryId == categoryId)
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<global::TaskNest.Models.TaskItem?> GetByIdAsync(string id)
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => t.Id == id && !t.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountActiveAsync()
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted)
            .CountAsync();
    }

    public async Task<int> CountCompletedAsync()
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted && t.IsCompleted)
            .CountAsync();
    }

    public async Task<int> CountActiveByCategoryAsync(string categoryId)
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted && t.CategoryId == categoryId)
            .CountAsync();
    }

    public async Task<int> AddAsync(global::TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        if (string.IsNullOrWhiteSpace(task.Id))
        {
            task.Id = Guid.NewGuid().ToString("N");
        }

        task.CreatedAtUtc = DateTime.UtcNow;
        task.UpdatedAtUtc = DateTime.UtcNow;
        task.IsDeleted = false;
        task.SyncStatus = SyncStatus.PendingCreate;

        return await db.InsertAsync(task);
    }

    public async Task<int> UpdateAsync(global::TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        task.UpdatedAtUtc = DateTime.UtcNow;

        if (task.SyncStatus != SyncStatus.PendingCreate)
        {
            task.SyncStatus = SyncStatus.PendingUpdate;
        }

        return await db.UpdateAsync(task);
    }

    public async Task<int> ClearCategoryAsync(string categoryId)
    {
        var db = await _database.GetConnectionAsync();

        var tasksToUpdate = await db.Table<global::TaskNest.Models.TaskItem>()
            .Where(t => !t.IsDeleted && t.CategoryId == categoryId)
            .ToListAsync();

        foreach (var task in tasksToUpdate)
        {
            task.CategoryId = null;
            task.UpdatedAtUtc = DateTime.UtcNow;

            if (task.SyncStatus != SyncStatus.PendingCreate)
            {
                task.SyncStatus = SyncStatus.PendingUpdate;
            }

            await db.UpdateAsync(task);
        }

        return tasksToUpdate.Count;
    }

    public async Task<int> SoftDeleteAsync(global::TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        task.IsDeleted = true;
        task.UpdatedAtUtc = DateTime.UtcNow;
        task.SyncStatus = SyncStatus.PendingDelete;

        return await db.UpdateAsync(task);
    }

    public async Task<int> DeleteAsync(global::TaskNest.Models.TaskItem task)
    {
        return await SoftDeleteAsync(task);
    }
}