using SQLite;
using TaskNest.Models;
using TaskNest.Models.Enums;

namespace TaskNest.Data;

public class AppDatabase
{
    private SQLiteAsyncConnection? _database;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database != null)
            return _database;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasknest.db");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<CategoryItem>();
        await _database.CreateTableAsync<TaskItem>();
        await EnsureTaskItemColumnsAsync(_database);

        await SeedDataAsync(_database);

        return _database;
    }

    private static async Task EnsureTaskItemColumnsAsync(SQLiteAsyncConnection db)
    {
        var taskItemColumns = await db.QueryAsync<TableInfoRow>("PRAGMA table_info('TaskItem')");
        var hasTaskColorHexColumn = taskItemColumns.Any(column =>
            string.Equals(column.Name, "TaskColorHex", StringComparison.OrdinalIgnoreCase));
        var hasReflectionColumn = taskItemColumns.Any(column =>
            string.Equals(column.Name, "Reflection", StringComparison.OrdinalIgnoreCase));

        if (!hasTaskColorHexColumn)
        {
            await db.ExecuteAsync("ALTER TABLE TaskItem ADD COLUMN TaskColorHex TEXT");
        }

        if (!hasReflectionColumn)
        {
            await db.ExecuteAsync("ALTER TABLE TaskItem ADD COLUMN Reflection TEXT");
        }
    }

    private static async Task SeedDataAsync(SQLiteAsyncConnection db)
    {
        var categoryCount = await db.Table<CategoryItem>().CountAsync();
        var taskCount = await db.Table<TaskItem>().CountAsync();

        if (categoryCount > 0 || taskCount > 0)
            return;

        var now = DateTime.UtcNow;

        var workCategory = new CategoryItem
        {
            Name = "Work",
            Description = "Work-related tasks",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            IsDeleted = false,
            SyncStatus = SyncStatus.PendingCreate
        };

        var studyCategory = new CategoryItem
        {
            Name = "Study",
            Description = "University and revision tasks",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            IsDeleted = false,
            SyncStatus = SyncStatus.PendingCreate
        };

        var personalCategory = new CategoryItem
        {
            Name = "Personal",
            Description = "Personal reminders and life admin",
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            IsDeleted = false,
            SyncStatus = SyncStatus.PendingCreate
        };

        await db.InsertAsync(workCategory);
        await db.InsertAsync(studyCategory);
        await db.InsertAsync(personalCategory);

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "Prepare project demo",
                Description = "Review core app screens before recording the demo.",
                DueDate = DateTime.Today.AddDays(2),
                TaskColorHex = "#EF4444",
                IsCompleted = false,
                CategoryId = workCategory.Id,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                IsDeleted = false,
                SyncStatus = SyncStatus.PendingCreate
            },
            new TaskItem
            {
                Title = "Revise database design",
                Description = "Go over SQLite integration and repository pattern notes.",
                DueDate = DateTime.Today.AddDays(3),
                TaskColorHex = "#F59E0B",
                IsCompleted = false,
                CategoryId = studyCategory.Id,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                IsDeleted = false,
                SyncStatus = SyncStatus.PendingCreate
            },
            new TaskItem
            {
                Title = "Buy groceries",
                Description = "Get essentials for the week.",
                DueDate = DateTime.Today.AddDays(1),
                TaskColorHex = "#22C55E",
                IsCompleted = false,
                CategoryId = personalCategory.Id,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                IsDeleted = false,
                SyncStatus = SyncStatus.PendingCreate
            }
        };

        await db.InsertAllAsync(tasks);
    }

    private sealed class TableInfoRow
    {
        public string Name { get; set; } = string.Empty;
    }
}