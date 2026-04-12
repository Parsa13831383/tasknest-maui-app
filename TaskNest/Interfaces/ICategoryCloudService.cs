using TaskNest.Models.Cloud;

namespace TaskNest.Interfaces;

public interface ICategoryCloudService
{
    Task<List<CloudCategoryDto>> GetCategoriesAsync();
    Task<CloudCategoryDto?> GetCategoryByIdAsync(string id);
    Task<List<CloudCategoryDto>> CreateCategoryAsync(CloudCategoryDto category);
    Task<List<CloudCategoryDto>> UpdateCategoryAsync(string id, CloudCategoryDto category);
    Task<List<CloudCategoryDto>> SoftDeleteCategoryAsync(string id);
}