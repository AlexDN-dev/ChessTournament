namespace BLL.Interfaces;

public interface IService<TDto, TCreateDto>
{
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto> GetByIdAsync(Guid id);
    Task<TDto> CreateAsync(TCreateDto dto);
    Task DeleteAsync(Guid id);
}