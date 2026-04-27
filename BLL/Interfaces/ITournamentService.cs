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

    // Command pur : met à jour le résultat uniquement
    Task UpdateEncounterAsync(Guid encounterId, string result);

    // Query : vérifie si tous les matchs de la ronde actuelle ont un résultat
    Task<bool> IsRoundCompleteAsync(Guid tournamentId);

    // Command : termine le tournoi si c'est la dernière ronde
    Task FinishTournamentIfLastRoundAsync(Guid tournamentId);

    Task NextRoundAsync(Guid tournamentId);
}
