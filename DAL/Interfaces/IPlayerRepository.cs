using Domain.Entities;

namespace DAL.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetPlayerByUsernameAsync(string username);
    Task<Player?> CreatePlayerAsync(Player p);
}
