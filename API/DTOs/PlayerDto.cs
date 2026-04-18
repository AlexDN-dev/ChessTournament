using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace API.DTOs;

public record CreatePlayerRequest(
    [Required(ErrorMessage = "Un pseudonyme est requis.")]
    string Username,

    [Required, MaxLength(200)]
    [EmailAddress(ErrorMessage = "Merci de rentrer un mail valide.")]
    string Email,

    [Required]
    [RegularExpression(PlayerConstants.PasswordRegex, ErrorMessage = PlayerConstants.PasswordRequirementsMessage)]
    string Password,

    [Required(ErrorMessage = "Une date de naissance est requise.")]
    DateTime Birthday,

    [Required(ErrorMessage = "Un genre est requis.")]
    string Gender,

    int? Elo = PlayerConstants.DefaultElo
);
