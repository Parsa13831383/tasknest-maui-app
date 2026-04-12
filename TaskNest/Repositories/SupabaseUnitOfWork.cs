using TaskNest.Interfaces;

namespace TaskNest.Repositories;

public class SupabaseUnitOfWork : IUnitOfWork
{
    public ITaskRepository Tasks { get; }
    public ICategoryRepository Categories { get; }

    public SupabaseUnitOfWork(ITaskRepository taskRepository, ICategoryRepository categoryRepository)
    {
        Tasks = taskRepository;
        Categories = categoryRepository;
    }

    public Task InitializeAsync() => Task.CompletedTask;
}