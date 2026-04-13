using TaskNest.Interfaces;

namespace TaskNest.Repositories;

// Coordinates repository access for the cloud-backed data layer.
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