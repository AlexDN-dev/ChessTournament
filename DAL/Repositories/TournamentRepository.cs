using DAL.Context;
using Domain.Entities;
using Domain.Filters;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly ChessTournamentContext _context;

    public TournamentRepository(ChessTournamentContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter)
    {
        var query = _context.Tournaments
            .Include(t => t.Categories)
            .AsNoTracking()
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(t => t.Name.Contains(filter.Name));
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            query = query.Where(t => t.Location.Contains(filter.Location));
        }
        
        if (filter.WomenOnly.HasValue)
        {
            query = query.Where(t => t.WomenOnly == filter.WomenOnly.Value);
        }
        
        int page = filter.Page <= 0 ? 1 : filter.Page;
        int pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }
}