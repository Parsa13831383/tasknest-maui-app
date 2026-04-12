namespace TaskNest.Interfaces;

public interface ICloudIdMapService
{
    int GetOrCreateTaskLocalId(string cloudId);
    int GetOrCreateCategoryLocalId(string cloudId);
    string? GetTaskCloudId(int localId);
    string? GetCategoryCloudId(int localId);
}