using BLL.DTOs;

namespace BLL.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<PlayerDto> GetPlayerByUsernameAsync(string username);
}