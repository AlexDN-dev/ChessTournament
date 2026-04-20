using BLL.Dtos;

namespace BLL.Interfaces;

public interface IScoreService
{
    Task<IEnumerable<PlayerScoreDto>> GetScoreTableAsync(Guid tournamentId, int? round);
}
