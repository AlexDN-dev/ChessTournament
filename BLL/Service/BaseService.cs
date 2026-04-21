using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Exceptions;

namespace BLL.Service;

public abstract class BaseService<TEntity, TDto, TCreateDto>
    : IService<TDto, TCreateDto>
    where TEntity : class
{
    protected readonly IRepository<TEntity> _repository;

    protected BaseService(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(ToDto);
    }

    public async Task<TDto> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException($"Entité introuvable avec l'id '{id}'.");

        return ToDto(entity);
    }

    public async Task<TDto> CreateAsync(TCreateDto dto)
    {
        var entity = ToEntity(dto);
        var created = await _repository.CreateAsync(entity);
        return ToDto(created);
    }

    public Task DeleteAsync(Guid id)
        => _repository.DeleteAsync(id);

    protected abstract TDto ToDto(TEntity entity);
    protected abstract TEntity ToEntity(TCreateDto dto);
}