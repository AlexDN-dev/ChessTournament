using API.DTOs;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories.Select(ToCategoryDto));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto)
    {
        await _service.CreateCategoryAsync(ToEntity(categoryDto));
        return StatusCode(201, "Catégorie créée avec succès");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteCategory(id);
        return Ok("La catégorie à bien été supprimé.");
    }

    private static CategoryDto ToCategoryDto(Category c)
        => new(c.Id, c.Name);

    private static Category ToEntity(CreateCategoryDto c)
    {
        return new Category
        {
            Name = c.Name
        };
    }
}