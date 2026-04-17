using Domain.Entities;
using Domain.Filters;

namespace BLL.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<Tournament>> GetAllAsync(TournamentFilter filter);
}