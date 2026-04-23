using Domain.Entities;

namespace DAL.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetPlayerByUsernameAsync(string username);
}
