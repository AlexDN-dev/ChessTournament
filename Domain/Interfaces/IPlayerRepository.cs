using Domain.Entities;

namespace Domain.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player> GetPlayerByUsernameAsync(string username);
    Task<Player> CreatePlayerAsync(Player p);
    Task<Player> IfPlayerExistAsync(string email, string username);
}