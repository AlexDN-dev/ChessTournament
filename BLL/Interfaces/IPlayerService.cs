using BLL.Dtos;

namespace BLL.Interfaces;

public interface IPlayerService : IService<PlayerDto, CreatePlayerDto>
{
    Task<PlayerDto> GetPlayerByUsernameAsync(string username);
    Task<LoginDto> LoginPlayerAsync(LoginPlayerDto dto);
}
