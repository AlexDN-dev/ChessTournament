using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record CategoryDto(
    [Required]
    Guid Id,
    [Required(ErrorMessage = "Le nom de la catégorie est requis.")]
    string Name);
    
public record CreateCategoryDto(
    [Required(ErrorMessage = "Le nom de la catégorie est requis.")]
    string Name);