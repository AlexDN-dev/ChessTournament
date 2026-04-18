using Domain.Entities;
using Domain.Filters;

namespace DAL.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter);
    Task<Tournament?> GetTournamentByIdAsync(Guid id);
    Guid CreateTournamentAsync(Tournament tournament, List<Guid> categoryIds);
    Task DeleteTournamentAsync(Guid id);
    Task RegisterPlayerToTournamentAsync(PlayerTournament pt);
    Task UnsubscribePlayerFromTournamentAsync(PlayerTournament pt);
    Task StartTournament(List<EncounterTournament> encounterTournaments, Guid tournamentId);
    Task<EncounterTournament> GetEncounterByIdAsync(Guid encounterId);
    Task UpdateEncounterAsync(Guid encounterId, string result);
    Task NextRoundAsync(Guid tournamentId);
    Task FinishTournamentAsync(Guid tournamentId);
}
