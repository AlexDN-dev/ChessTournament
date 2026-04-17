using Domain.Entities;
using Domain.Filters;

namespace Domain.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter);
}