using SQLite;
using TaskNest.Models;

namespace TaskNest.Data;

public class AppDatabase
{
    private SQLiteAsyncConnection? _database;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database != null)
            return _database;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasknest.db");
        System.Diagnostics.Debug.WriteLine($"DB PATH: {dbPath}");

        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<TaskItem>();
        await _database.CreateTableAsync<CategoryItem>();

        return _database;
    }
}