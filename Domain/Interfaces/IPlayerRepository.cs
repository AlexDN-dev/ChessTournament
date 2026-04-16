using Domain.Entities;

namespace Domain.Interfaces;

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player> GetPlayerByUsernameAsync(string username);
}