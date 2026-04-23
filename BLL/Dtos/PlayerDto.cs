namespace BLL.Dtos;

public record PlayerDto(
    Guid Id,
    string Username,
    string Email,
    DateTime Birthday,
    string Gender,
    int? Elo);

public record PlayerSummaryDto(Guid Id, string Username);

public record CreatePlayerDto(
    string Username,
    string Email,
    DateTime Birthday,
    string Gender,
    int? Elo);

public record LoginPlayerDto(
    string Username,
    string Password);

public record LoginDto(
    string Username,
    string Role,
    int Elo,
    string Token,
    DateTime ExpirateAt
);
