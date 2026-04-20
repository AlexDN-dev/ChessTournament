using System.Text.Json;
using DAL.Context;
using DAL.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
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

    public async Task<Player?> GetPlayerByUsernameAsync(string username)
    {
        return await _context.Players.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Username == username);
    }

    public async Task<Player?> CreatePlayerAsync(Player p)
    {
        var json = JsonSerializer.Serialize(p);
        var param = new SqlParameter("@Json", json);

        try
        {
            // AsEnumerable() forces client-side evaluation: EXEC is non-composable, EF can't add WHERE/ORDER on top of it
            return _context.Players
                .FromSqlRaw("EXEC dbo.AddPlayer @Json", param)
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault();
        }
        catch (SqlException ex)
        {
            throw MapSqlException(ex);
        }
    }

    private static ChessTournamentException MapSqlException(SqlException ex)
    {
        var message = ex.Message ?? string.Empty;

        if (message.Contains("username", StringComparison.OrdinalIgnoreCase) && message.Contains("utilisé", StringComparison.OrdinalIgnoreCase))
            return new ConflictException("Ce pseudo est déjà utilisé.", ex);

        if (message.Contains("email", StringComparison.OrdinalIgnoreCase) && message.Contains("utilisé", StringComparison.OrdinalIgnoreCase))
            return new ConflictException("Cet email est déjà utilisé.", ex);

        if (message.Contains("naissance", StringComparison.OrdinalIgnoreCase))
            return new ValidationException("La date de naissance doit être dans le passé.", ex);

        if (message.Contains("champs sont obligatoires", StringComparison.OrdinalIgnoreCase))
            return new ValidationException("Tous les champs sont obligatoires.", ex);

        if (message.Contains("JSON", StringComparison.OrdinalIgnoreCase))
            return new ValidationException("Le payload envoyé à la base est invalide.", ex);

        return new ConflictException("Erreur lors de la création du joueur.", ex);
    }
}
