using DAL.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly ChessTournamentContext _context;

    public PlayerRepository(ChessTournamentContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        return await _context.Players.AsNoTracking().ToListAsync();
    }

    public async Task<Player> GetPlayerByUsernameAsync(string username)
    {
        return await _context.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Username == username);
    }
}