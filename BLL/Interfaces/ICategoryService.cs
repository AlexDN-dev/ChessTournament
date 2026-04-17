using Domain.Entities;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<int> CreateCategoryAsync(Category category);
    Task<int> DeleteCategory(Guid id);
}