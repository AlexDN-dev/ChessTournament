using BLL.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace BLL.Service;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories;
    }

    public Task<int> CreateCategoryAsync(Category category)
    {
        return _repository.CreateCategoryAsync(category);

    }

    public async Task<int> DeleteCategory(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new Exception("L'id n'est pas correct");
        }
        return await _repository.DeleteCategoryAsync(id);
    }
}