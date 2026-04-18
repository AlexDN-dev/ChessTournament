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

public record EncounterDto(
    Guid Id,
    string Player1Username,
    string Player2Username,
    string? Result,
    DateTime EncounterDate);

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
    IEnumerable<PlayerSummaryDto> RegisteredPlayers,
    IEnumerable<EncounterDto> CurrentRoundEncounters);

public record CreateTournamentDto(
    string Name,
    string Location,
    int MinPlayer,
    int MaxPlayer,
    int? MinElo,
    int? MaxElo,
    bool WomenOnly,
    DateTime? FinalRegisterDate,
    List<Guid> CategoryIds
);
