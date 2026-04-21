using DAL.Context;
using DAL.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ChessTournamentContext context) : base(context) { }

    public override async Task<Category> CreateAsync(Category category)
    {
        bool exists = await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == category.Name.Trim().ToLower());
        if (exists)
            throw new ConflictException($"Une catégorie nommée '{category.Name}' existe déjà.");

        return await base.CreateAsync(category);
    }
}
