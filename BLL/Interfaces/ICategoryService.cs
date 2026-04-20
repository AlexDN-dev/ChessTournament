using BLL.Dtos;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task DeleteCategoryAsync(Guid id);
}
