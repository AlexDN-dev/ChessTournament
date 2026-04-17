using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record TournamentDto(
    Guid Id,
    string Name,
    string Location,
    int MinPlayer,
    int MaxPlayer,
    int MinElo,
    int MaxElo,
    string Status,
    int ActualRound,
    bool WomenOnly,
    DateTime FinalRegisterDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<CategoryDto> Categories
    );
    
public record TournamentPaginationDto(
    bool? WomenOnly,

    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    string? Location,

    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    string? Name,
    [Range(1, int.MaxValue, ErrorMessage = "Il doit y avoir au minimum 1 page.")]
    int Page = 1,

    [Range(1, 50, ErrorMessage = "PageSize doit être compris entre 1 et 50.")]
    int PageSize = 10
);