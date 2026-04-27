using API.DTOs;
using BLL.Dtos;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryRequest request)
    {
        // Command : retourne seulement l'Id
        var id = await _service.CreateAsync(new CreateCategoryDto(request.Name, request.MinAge, request.MaxAge));
        // Query : récupère le DTO complet pour la réponse 201
        var created = await _service.GetByIdAsync(id);
        return CreatedAtAction(nameof(GetAll), new { id = id }, created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
