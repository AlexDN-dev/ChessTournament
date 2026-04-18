using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Exceptions;
using Domain.Filters;

namespace BLL.Service;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _repository;

    public TournamentService(ITournamentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TournamentSummaryDto>> GetAllAsync(TournamentFilter filter)
    {
        var tournaments = await _repository.GetAllAsync(filter);
        return tournaments.Select(t => t.ToSummaryDto());
    }

    public async Task<TournamentDetailDto> GetTournamentByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("L'identifiant du tournoi est invalide.");

        var tournament = await _repository.GetTournamentByIdAsync(id);
        if (tournament is null)
            throw new NotFoundException($"Aucun tournoi trouvé avec l'identifiant '{id}'.");

        return tournament.ToDetailDto();
    }
}
