using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record CreateCategoryRequest(
    [Required(ErrorMessage = "Le nom de la catégorie est requis.")]
    [StringLength(50, MinimumLength = 1)]
    string Name);
