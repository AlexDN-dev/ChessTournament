using DAL.Context;
using DAL.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ChessTournamentContext _context;

    public CategoryRepository(ChessTournamentContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        var normalized = category.Name.Trim().ToLower();
        bool exists = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == normalized);
        if (exists)
            throw new ConflictException($"Une catégorie nommée '{category.Name}' existe déjà.");

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
            throw new NotFoundException($"Aucune catégorie trouvée avec l'identifiant '{id}'.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
