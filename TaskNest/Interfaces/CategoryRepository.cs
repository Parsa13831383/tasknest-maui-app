using TaskNest.Data;
using TaskNest.Interfaces;
using TaskNest.Models;

namespace TaskNest.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDatabase _database;

    public CategoryRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<CategoryItem>> GetAllAsync()
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<CategoryItem>()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CategoryItem?> GetByIdAsync(int id)
    {
        var db = await _database.GetConnectionAsync();

        return await db.Table<CategoryItem>()
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(CategoryItem category)
    {
        var db = await _database.GetConnectionAsync();

        category.CreatedAtUtc = DateTime.UtcNow;
        category.UpdatedAtUtc = DateTime.UtcNow;
        category.IsDeleted = false;

        return await db.InsertAsync(category);
    }

    public async Task<int> UpdateAsync(CategoryItem category)
    {
        var db = await _database.GetConnectionAsync();

        category.UpdatedAtUtc = DateTime.UtcNow;

        return await db.UpdateAsync(category);
    }

    public async Task<int> DeleteAsync(CategoryItem category)
    {
        var db = await _database.GetConnectionAsync();

        category.IsDeleted = true;
        category.UpdatedAtUtc = DateTime.UtcNow;

        return await db.UpdateAsync(category);
    }
}