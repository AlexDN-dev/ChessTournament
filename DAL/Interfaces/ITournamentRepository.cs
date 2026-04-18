using Domain.Entities;
using Domain.Filters;

namespace DAL.Interfaces;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter);
    Task<Tournament?> GetTournamentByIdAsync(Guid id);
}
