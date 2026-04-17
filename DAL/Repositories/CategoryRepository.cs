using DAL.Context;
using Domain.Entities;
using Domain.Interfaces;
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

    public async Task<int> CreateCategoryAsync(Category category)
    {
        var cat = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());
        if (cat is not null)
        {
            throw new Exception("Une catégorie avec le même nom existe déjà.");
        }

        await _context.Categories.AddAsync(category);
        int response = await _context.SaveChangesAsync();
        return response;
    }

    public async Task<int> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            throw new Exception("Cette catégorie n'existe pas.");
        }

        _context.Categories.Remove(category);
        int response = await _context.SaveChangesAsync();
        return response;
    }
}