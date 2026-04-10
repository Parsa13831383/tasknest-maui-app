using TaskNest.Data;
using TaskNest.Interfaces;

namespace TaskNest.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDatabase _database;

    public TaskRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<TaskNest.Models.TaskItem>> GetAllAsync()
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<TaskNest.Models.TaskItem>()
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<TaskNest.Models.TaskItem?> GetByIdAsync(int id)
    {
        var db = await _database.GetConnectionAsync();

        return await db.FindAsync<TaskNest.Models.TaskItem>(id);
    }

    public async Task<int> AddAsync(TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        return await db.InsertAsync(task);
    }

    public async Task<int> UpdateAsync(TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        return await db.UpdateAsync(task);
    }

    public async Task<int> DeleteAsync(TaskNest.Models.TaskItem task)
    {
        var db = await _database.GetConnectionAsync();

        return await db.DeleteAsync(task);
    }
}