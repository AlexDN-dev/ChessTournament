using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public record PlayerDto(
    Guid Id, 
    [Required]
    string Username,
    [Required, EmailAddress, MaxLength(200)]
    string Email, 
    DateTime Birthday, 
    string Gender, 
    int? Elo);