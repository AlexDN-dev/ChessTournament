using DAL.Context;
using DAL.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Filters;
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
            .Include(t => t.PlayerTournaments)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(t => t.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(t => t.Location.Contains(filter.Location));

        if (filter.WomenOnly.HasValue)
            query = query.Where(t => t.WomenOnly == filter.WomenOnly.Value);

        int page = filter.Page <= 0 ? 1 : filter.Page;
        int pageSize = filter.PageSize <= 0
            ? TournamentConstants.DefaultPageSize
            : Math.Min(filter.PageSize, TournamentConstants.MaxPageSize);

        query = query
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<Tournament?> GetTournamentByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Categories)
            .Include(t => t.PlayerTournaments)
            .ThenInclude(pt => pt.Player)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
