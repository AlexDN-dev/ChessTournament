using BLL.Dtos;
using Domain.Entities;

namespace BLL.Mappers;

internal static class PlayerMapper
{
    public static PlayerDto ToDto(this Player p)
        => new(p.Id, p.Username, p.Email, p.Birthday, p.Gender, p.Elo);

    public static PlayerSummaryDto ToSummaryDto(this Player p)
        => new(p.Id, p.Username);

    public static Player ToEntity(this CreatePlayerDto dto, string password)
        => new()
        {
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = password,
            Birthday = dto.Birthday,
            Gender = dto.Gender,
            Elo = dto.Elo
        };

    public static LoginDto toLoginDto(this Player p, string token)
        => new(
            p.Username,
            p.Role,
            p.Elo ?? 1200,
            token,
            DateTime.UtcNow.AddHours(1)
        );
}
