using System.Text.RegularExpressions;
using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Constants;
using Domain.Exceptions;

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
        return players.Select(p => p.ToDto());
    }

    public async Task<PlayerDto> GetPlayerByUsernameAsync(string username)
    {
        var player = await _repository.GetPlayerByUsernameAsync(username);
        if (player is null)
            throw new NotFoundException($"Aucun joueur trouvé avec le pseudo '{username}'.");

        return player.ToDto();
    }

    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
    {
        if (!PlayerConstants.AllowedGenders.Contains(dto.Gender))
            throw new ValidationException($"Genre invalide. Valeurs autorisées : {string.Join(", ", PlayerConstants.AllowedGenders)}.");

        if (!Regex.IsMatch(dto.Password, PlayerConstants.PasswordRegex))
            throw new ValidationException(PlayerConstants.PasswordRequirementsMessage);

        var entity = dto.ToEntity();
        entity.HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var created = await _repository.CreatePlayerAsync(entity);
        return created!.ToDto();
    }
}
