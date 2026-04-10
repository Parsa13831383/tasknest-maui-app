using TaskNest.Data;
using TaskNest.Interfaces;

namespace TaskNest.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDatabase _database;

    public ITaskRepository Tasks { get; }
    public ICategoryRepository Categories { get; }

    public UnitOfWork(
        AppDatabase database,
        ITaskRepository taskRepository,
        ICategoryRepository categoryRepository)
    {
        _database = database;
        Tasks = taskRepository;
        Categories = categoryRepository;
    }

    public async Task InitializeAsync()
    {
        await _database.GetConnectionAsync();
    }
}