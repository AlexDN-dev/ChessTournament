using System.Text.Json;
using DAL.Context;
using DAL.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enum;
using Domain.Filters;
using Microsoft.Data.SqlClient;
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
            .Include(t => t.EncounterTournaments)
                .ThenInclude(e => e.Player1Navigation)
            .Include(t => t.EncounterTournaments)
                .ThenInclude(e => e.Player2Navigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public Guid CreateTournamentAsync(Tournament tournament, List<Guid> categoryIds)
    {
        var json = JsonSerializer.Serialize(new {
            tournament.Name, tournament.Location,
            tournament.MinPlayer, tournament.MaxPlayer,
            tournament.MinElo, tournament.MaxElo,
            tournament.WomenOnly, tournament.FinalRegisterDate,
            CategoryIds = categoryIds
        });
        var param = new SqlParameter("@Json", json);

        return _context.Database
            .SqlQuery<Guid>($"EXEC dbo.AddTournament {param}")
            .AsEnumerable()
            .First();
    }

    public async Task DeleteTournamentAsync(Guid id)
    {
        Tournament? t = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == id);
        _context.Tournaments.Remove(t);
        await _context.SaveChangesAsync();
        
    }

    public async Task RegisterPlayerToTournamentAsync(PlayerTournament pt)
    { 
        _context.PlayerTournaments.Add(pt);
        await _context.SaveChangesAsync();
    }

     public async Task UnsubscribePlayerFromTournamentAsync(PlayerTournament pt)
    {
        _context.PlayerTournaments.Remove(pt);
        await _context.SaveChangesAsync();
    }

    public async Task StartTournament(List<EncounterTournament> encounterTournaments, Guid tournamentId)
    {
        Tournament? t = await _context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
        t.ActualRound = 1;
        t.UpdatedAt = DateTime.Now;
        t.Status = TournamentStatus.STARTED;
        _context.Tournaments.Update(t);
        _context.EncounterTournaments.AddRange(encounterTournaments);
        await _context.SaveChangesAsync();
    }

    public async Task<EncounterTournament> GetEncounterByIdAsync(Guid encounterId)
    {
        EncounterTournament? et = await _context.EncounterTournaments.FirstOrDefaultAsync(et => et.Id == encounterId);
        return et;
    }

    public async Task UpdateEncounterAsync(Guid encounterId, string result)
    {
        EncounterTournament? et = await _context.EncounterTournaments.FirstOrDefaultAsync(et => et.Id == encounterId);
        et.Result = result;
        await _context.SaveChangesAsync();
    }
}
