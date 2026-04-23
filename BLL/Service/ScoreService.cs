using BLL.Dtos;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Enum;
using Domain.Exceptions;

namespace BLL.Service;

public class ScoreService : IScoreService
{
    private readonly ITournamentRepository _repository;

    public ScoreService(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PlayerScoreDto>> GetScoreTableAsync(Guid tournamentId, int? round)
    {
        if (tournamentId == Guid.Empty)
            throw new ValidationException("L'identifiant du tournoi est invalide.");

        var tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException($"Aucun tournoi trouvé avec l'identifiant '{tournamentId}'.");

        if (tournament.Status == TournamentStatus.PENDING)
            throw new ValidationException("Le tournoi n'a pas encore commencé, aucun score disponible.");

        if (round.HasValue)
        {
            if (round.Value < 1)
                throw new ValidationException("Le numéro de ronde doit être supérieur ou égal à 1.");

            if (round.Value > tournament.ActualRound)
                throw new ValidationException(
                    $"La ronde {round.Value} n'a pas encore eu lieu. Ronde courante : {tournament.ActualRound}.");
        }

        var encounters = tournament.EncounterTournaments
            .Where(e => e.Result != null && (!round.HasValue || e.Round <= round.Value))
            .ToList();

        var scores = tournament.PlayerTournaments.Select(pt =>
        {
            var playerId = pt.PlayerId;

            var playerEncounters = encounters
                .Where(e => e.Player1 == playerId || e.Player2 == playerId)
                .ToList();

            int wins = playerEncounters.Count(e =>
                (e.Player1 == playerId && e.Result == "1-0") ||
                (e.Player2 == playerId && e.Result == "0-1"));

            int losses = playerEncounters.Count(e =>
                (e.Player1 == playerId && e.Result == "0-1") ||
                (e.Player2 == playerId && e.Result == "1-0"));

            int draws = playerEncounters.Count(e => e.Result == "1/2-1/2");

            double score = wins + draws * 0.5;

            return new PlayerScoreDto(pt.Player.Username, playerEncounters.Count, wins, losses, draws, score);
        })
        .OrderByDescending(s => s.Score)
        .ToList();

        return scores;
    }
}
