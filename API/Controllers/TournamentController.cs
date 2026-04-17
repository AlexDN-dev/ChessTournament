using API.DTOs;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Filters;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _service;

    public TournamentController(ITournamentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPagination([FromQuery] TournamentPaginationDto request)
    {
        var filter = new TournamentFilter
        {
            Page = request.Page < 1 ? 1 : request.Page,
            PageSize = request.PageSize > 50 ? 50 : request.PageSize,
            WomenOnly = request.WomenOnly,
            Location = request.Location,
            Name = request.Name
        };
        var tournaments = await _service.GetAllAsync(filter);
        return Ok(tournaments.Select(ToTournamentDto));
    }
    private static CategoryDto ToCategoryDto(Category c)
        => new(c.Id, c.Name);
    
    private static TournamentDto ToTournamentDto(Tournament t)
        => new(
            t.Id, 
            t.Name, 
            t.Location, 
            t.MinPlayer, 
            t.MaxPlayer, 
            t.MinElo ?? 0, 
            t.MaxElo ?? 0, 
            t.Status, 
            t.ActualRound ?? 0, 
            t.WomenOnly, 
            t.FinalRegisterDate ?? new DateTime(), 
            t.CreatedAt ?? new DateTime(), 
            t.UpdatedAt ?? new DateTime(), 
            t.Categories.Select(ToCategoryDto));
}