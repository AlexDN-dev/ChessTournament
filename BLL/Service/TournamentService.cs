using BLL.Interfaces;
using Domain.Entities;
using Domain.Filters;
using Domain.Interfaces;

namespace BLL.Service;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _repository;

    public TournamentService(ITournamentRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter)
    {
        var tournaments = await _repository.GetAllAsync(filter);
        return tournaments;
    }

    public async Task<Tournament> GetTournamentById(Guid id)
    {
        if (id == Guid.Empty)
            throw new Exception("Id invalide.");
        var tournament = await _repository.GetTournamentById(id);
        return tournament;
    }
}