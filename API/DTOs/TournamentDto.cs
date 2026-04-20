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

public record CreateTournamentRequest(
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    string Name,
    [StringLength(150, ErrorMessage = "Le nom ne peut pas dépasser 150 caractères.")]
    string Location,
    [Range(4, 20, ErrorMessage = "Il Faut au moins 4 joueurs pour que le tournoi soit validé.")]
    int MinPlayer,
    [Range(4, 20, ErrorMessage = "Le maximum de joueur est 20.")]
    int MaxPlayer,
    int? MinElo,
    int? MaxElo,
    [Required(ErrorMessage = "Une date de fin d'inscription est obligatoire.")]
    DateTime FinalRegisterDate,
    [MinLength(1), MaxLength(2), Required(ErrorMessage = "Il faut au moins 1 catégorie, au maximum 2.")]
    List<Guid> CategoryIds,
    bool WomenOnly = false
    
    
);
