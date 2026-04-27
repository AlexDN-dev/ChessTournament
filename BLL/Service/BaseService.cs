using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace BLL.Service;

public abstract class BaseService<TEntity, TDto, TCreateDto>
    : IService<TDto, TCreateDto>
    where TEntity : class, IEntity
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

    public virtual async Task<Guid> CreateAsync(TCreateDto dto)
    {
        var entity = ToEntity(dto);
        var created = await _repository.CreateAsync(entity);
        return created.Id;
    }

    public Task DeleteAsync(Guid id)
        => _repository.DeleteAsync(id);

    protected abstract TDto ToDto(TEntity entity);
    protected abstract TEntity ToEntity(TCreateDto dto);
}
