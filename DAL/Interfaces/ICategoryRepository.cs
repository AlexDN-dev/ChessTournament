using Domain.Entities;

namespace DAL.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> CreateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Guid id);
}
