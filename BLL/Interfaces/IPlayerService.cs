using BLL.Dtos;

namespace BLL.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<PlayerDto> GetPlayerByUsernameAsync(string username);
    Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto);
}
