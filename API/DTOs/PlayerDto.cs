using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record PlayerDto(
    Guid Id, 
    [Required (ErrorMessage = "Un pseudonyme est requis.")]
    string Username,
    [Required,MaxLength(200)]
    [EmailAddress(ErrorMessage = "Merci de rentrer un mail valide.")]
    string Email,
    [Required (ErrorMessage = "Une date de naissance est requise.")]
    DateTime Birthday,
    [Required (ErrorMessage = "Un genre est requis.")]
    string Gender, 
    int? Elo);
    
public record CreatePlayerDto(
    [Required (ErrorMessage = "Un pseudonyme est requis.")]
    string Username,
    [Required,MaxLength(200)]
    [EmailAddress(ErrorMessage = "Merci de rentrer un mail valide.")]
    string Email,
    [Required]
    [RegularExpression(@"^(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Merci de rentrer au moins 8 caractères, un chiffre et un caractère spécial.")]
    string Password,
    [Required (ErrorMessage = "Une date de naissance est requise.")]
    DateTime BirthDay,
    [Required (ErrorMessage = "Un genre est requis.")]
    string Gender,
    int Elo
    );