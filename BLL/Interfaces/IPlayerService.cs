using Domain.Entities;

namespace BLL.Interfaces;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player> GetPlayerByUsernameAsync(string username);
    Task<Player> CreatePlayerAsync(Player player);
}