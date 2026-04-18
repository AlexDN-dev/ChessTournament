using BLL.Dtos;
using Domain.Filters;

namespace BLL.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<TournamentSummaryDto>> GetAllAsync(TournamentFilter filter);
    Task<TournamentDetailDto> GetTournamentByIdAsync(Guid id);
    Task<Guid> CreateTournamentAsync(CreateTournamentDto dto);
}
