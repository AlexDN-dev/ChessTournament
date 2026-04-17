using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<int> CreateCategoryAsync(Category category);
    Task<int> DeleteCategoryAsync(Guid id);
}