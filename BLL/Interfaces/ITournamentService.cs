using BLL.Dtos;
using Domain.Filters;

namespace BLL.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<TournamentSummaryDto>> GetAllAsync(TournamentFilter filter);
    Task<TournamentDetailDto> GetTournamentByIdAsync(Guid id);
    Task<Guid> CreateTournamentAsync(CreateTournamentDto dto);
    Task DeleteTournamentAsync(Guid id);
    Task RegisterPlayerToTournamentAsync(string playerUsername, Guid tournamentId);
    Task UnsubscribePlayerFromTournamentAsync(string playerUsername, Guid tournamentId);
    Task StartTournamentAsync(Guid tournamentId);
    Task UpdateEncounterAsync(Guid encounterId, string result);
    Task NextRoundAsync(Guid tournamentId);
}
