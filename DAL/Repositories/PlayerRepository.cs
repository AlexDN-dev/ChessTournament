using System.Text.Json;
using DAL.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
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

    public async Task<Player> CreatePlayerAsync(Player p)
    {
        var json = JsonSerializer.Serialize(p);
        var param = new SqlParameter("@Json", json);

        try
        {
            var insertedPlayer = _context.Players
                .FromSqlRaw("EXEC dbo.AddPlayer @Json", param)
                .AsEnumerable()
                .FirstOrDefault();

            return insertedPlayer;
        }
        catch (SqlException ex)
        {
            var message = ex.Message;

            throw new Exception(message);
        }
    }

    public async Task<Player> IfPlayerExistAsync(string email, string username)
    {
        return await _context.Players.FirstOrDefaultAsync(p => p.Username == username || p.Email == email);
    }
}