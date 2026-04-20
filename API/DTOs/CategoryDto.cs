using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record CreateCategoryRequest(
    [Required(ErrorMessage = "Le nom de la catégorie est requis.")]
    [StringLength(50, MinimumLength = 1)]
    string Name,
    [Required]
    [Range(1, 100, ErrorMessage = "L'age doit être compris entre 1 et 100.")]
    int MinAge,
    [Required]
    [Range(1, 100, ErrorMessage = "L'age doit être compris entre 1 et 100.")]
    int MaxAge);
