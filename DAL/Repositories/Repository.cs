using DAL.Context;
using DAL.Interfaces;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ChessTournamentContext _context;

    public Repository(ChessTournamentContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _context.Set<T>().AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _context.Set<T>().FindAsync(id);

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
            throw new NotFoundException($"Entité introuvable avec l'id '{id}'.");

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}