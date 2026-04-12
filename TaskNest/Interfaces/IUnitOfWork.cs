namespace TaskNest.Interfaces;

public interface IUnitOfWork
{
    ITaskRepository Tasks { get; }
    ICategoryRepository Categories { get; }
    Task InitializeAsync();
}