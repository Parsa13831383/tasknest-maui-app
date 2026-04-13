using TaskNest.Interfaces;
using TaskNest.Models;
using TaskNest.Models.Cloud;
using TaskNest.Models.Enums;

namespace TaskNest.Repositories;

// Implements repository operations for categories using cloud-backed data services.
public class SupabaseCategoryRepository : ICategoryRepository
{
    private readonly ICategoryCloudService _categoryCloudService;
    private readonly ISupabaseAuthService _authService;

    public SupabaseCategoryRepository(
        ICategoryCloudService categoryCloudService,
        ISupabaseAuthService authService)
    {
        _categoryCloudService = categoryCloudService;
        _authService = authService;
    }

    public async Task<List<CategoryItem>> GetAllAsync()
    {
        EnsureAuthenticated();

        var cloudCategories = await _categoryCloudService.GetCategoriesAsync();
        return cloudCategories.Select(MapToCategoryItem).OrderBy(c => c.Name).ToList();
    }

    public async Task<CategoryItem?> GetByIdAsync(string id)
    {
        EnsureAuthenticated();

        var cloudCategory = await _categoryCloudService.GetCategoryByIdAsync(id);
        return cloudCategory is null ? null : MapToCategoryItem(cloudCategory);
    }

    public async Task<int> AddAsync(CategoryItem category)
    {
        EnsureAuthenticated();

        var created = await _categoryCloudService.CreateCategoryAsync(new CloudCategoryDto
        {
            UserId = _authService.UserId ?? string.Empty,
            Name = category.Name,
            Description = category.Description
        });

        var createdItem = created.FirstOrDefault();
        if (createdItem?.Id is null)
        {
            return 0;
        }

        category.Id = createdItem.Id ?? string.Empty;
        category.CreatedAtUtc = createdItem.CreatedAtUtc;
        category.UpdatedAtUtc = createdItem.UpdatedAtUtc;
        category.SyncStatus = SyncStatus.Synced;
        category.IsDeleted = false;
        return 1;
    }

    public async Task<int> UpdateAsync(CategoryItem category)
    {
        EnsureAuthenticated();

        if (string.IsNullOrWhiteSpace(category.Id))
        {
            return 0;
        }

        var updated = await _categoryCloudService.UpdateCategoryAsync(category.Id, new CloudCategoryDto
        {
            Name = category.Name,
            Description = category.Description
        });

        var updatedItem = updated.FirstOrDefault();
        if (updatedItem is null)
        {
            return 0;
        }

        category.UpdatedAtUtc = updatedItem.UpdatedAtUtc;
        category.SyncStatus = SyncStatus.Synced;
        return 1;
    }

    public async Task<int> DeleteAsync(CategoryItem category)
    {
        EnsureAuthenticated();

        if (string.IsNullOrWhiteSpace(category.Id))
        {
            return 0;
        }

        var deleted = await _categoryCloudService.SoftDeleteCategoryAsync(category.Id);
        if (!deleted.Any())
        {
            return 0;
        }

        category.IsDeleted = true;
        category.UpdatedAtUtc = DateTime.UtcNow;
        category.SyncStatus = SyncStatus.Synced;
        return 1;
    }

    private CategoryItem MapToCategoryItem(CloudCategoryDto cloudCategory)
    {
        return new CategoryItem
        {
            Id = cloudCategory.Id ?? string.Empty,
            Name = cloudCategory.Name,
            Description = cloudCategory.Description,
            CreatedAtUtc = cloudCategory.CreatedAtUtc,
            UpdatedAtUtc = cloudCategory.UpdatedAtUtc,
            IsDeleted = cloudCategory.IsDeleted,
            SyncStatus = SyncStatus.Synced
        };
    }

    private void EnsureAuthenticated()
    {
        if (!_authService.IsAuthenticated)
        {
            throw new InvalidOperationException("Please log in before accessing cloud data.");
        }
    }
}