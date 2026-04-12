using TaskNest.Interfaces;

namespace TaskNest.Services.Supabase;

public sealed class CloudIdMapService : ICloudIdMapService
{
    private readonly Dictionary<string, int> _taskCloudToLocal = new(StringComparer.Ordinal);
    private readonly Dictionary<int, string> _taskLocalToCloud = new();
    private readonly Dictionary<string, int> _categoryCloudToLocal = new(StringComparer.Ordinal);
    private readonly Dictionary<int, string> _categoryLocalToCloud = new();

    private int _nextTaskLocalId = 1;
    private int _nextCategoryLocalId = 1;

    public int GetOrCreateTaskLocalId(string cloudId)
    {
        if (_taskCloudToLocal.TryGetValue(cloudId, out var existing))
        {
            return existing;
        }

        var newId = _nextTaskLocalId++;
        _taskCloudToLocal[cloudId] = newId;
        _taskLocalToCloud[newId] = cloudId;
        return newId;
    }

    public int GetOrCreateCategoryLocalId(string cloudId)
    {
        if (_categoryCloudToLocal.TryGetValue(cloudId, out var existing))
        {
            return existing;
        }

        var newId = _nextCategoryLocalId++;
        _categoryCloudToLocal[cloudId] = newId;
        _categoryLocalToCloud[newId] = cloudId;
        return newId;
    }

    public string? GetTaskCloudId(int localId)
        => _taskLocalToCloud.TryGetValue(localId, out var cloudId) ? cloudId : null;

    public string? GetCategoryCloudId(int localId)
        => _categoryLocalToCloud.TryGetValue(localId, out var cloudId) ? cloudId : null;
}