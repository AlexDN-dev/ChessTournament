using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace API.DTOs;

public record TournamentPaginationRequest(
    bool? WomenOnly,

    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    string? Location,

    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    string? Name,

    [Range(1, int.MaxValue, ErrorMessage = "La page doit être supérieure ou égale à 1.")]
    int Page = 1,

    [Range(1, TournamentConstants.MaxPageSize, ErrorMessage = "PageSize doit être compris entre 1 et 50.")]
    int PageSize = TournamentConstants.DefaultPageSize
);
