using Domain.Enum;

namespace BLL.Dtos;

public record TournamentSummaryDto(
    Guid Id,
    string Name,
    string Location,
    int MinPlayer,
    int MaxPlayer,
    int? MinElo,
    int? MaxElo,
    TournamentStatus Status,
    int? ActualRound,
    bool WomenOnly,
    DateTime? FinalRegisterDate,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<CategoryDto> Categories,
    int RegisteredPlayerCount);

public record TournamentDetailDto(
    Guid Id,
    string Name,
    string Location,
    int MinPlayer,
    int MaxPlayer,
    int? MinElo,
    int? MaxElo,
    TournamentStatus Status,
    int? ActualRound,
    bool WomenOnly,
    DateTime? FinalRegisterDate,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<CategoryDto> Categories,
    IEnumerable<PlayerSummaryDto> RegisteredPlayers);
