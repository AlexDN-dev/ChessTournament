using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Exceptions;

namespace BLL.Service;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => c.ToDto());
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Le nom de la catégorie est requis.");

        var entity = dto.ToEntity();
        var created = await _repository.CreateCategoryAsync(entity);
        return created.ToDto();
    }

    public Task DeleteCategoryAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("L'identifiant de la catégorie est invalide.");

        return _repository.DeleteCategoryAsync(id);
    }
}
