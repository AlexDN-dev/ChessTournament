using BLL.DTOs;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace BLL.Service;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;

    public PlayerService(IPlayerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        var players = await _repository.GetAllAsync();
        return players.Select(ToDto);
    }

    public async Task<PlayerDto> GetPlayerByUsernameAsync(string username)
    {
        var player = await _repository.GetPlayerByUsernameAsync(username);
        if (player is null)
        {
            throw new KeyNotFoundException("Aucun joueur trouvé avec ce pseudo.");
        }

        return ToDto(player);
    }

    private static PlayerDto ToDto(Player p)
        => new(p.Id, p.Username, p.Email, p.Birthday, p.Gender, p.Elo);

}