using API.DTOs;
using BLL.Dtos;
using BLL.Interfaces;
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
    public async Task<ActionResult<IEnumerable<TournamentSummaryDto>>> GetPagination(
        [FromQuery] TournamentPaginationRequest request)
    {
        var filter = new TournamentFilter
        {
            Page       = request.Page,
            PageSize   = request.PageSize,
            WomenOnly  = request.WomenOnly,
            Location   = request.Location,
            Name       = request.Name
        };

        var tournaments = await _service.GetAllAsync(filter);
        return Ok(tournaments);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TournamentDetailDto>> GetTournament(Guid id)
    {
        var tournament = await _service.GetTournamentByIdAsync(id);
        return Ok(tournament);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDetailDto>> CreateTournament(CreateTournamentRequest request)
    {
        var input = new CreateTournamentDto
        (
            request.Name,
            request.Location,
            request.MinPlayer,
            request.MaxPlayer,
            request.MinElo,
            request.MaxElo,
            request.WomenOnly,
            request.FinalRegisterDate,
            request.CategoryIds
            );

        var tournamentId = await _service.CreateTournamentAsync(input);
        var tournament = await _service.GetTournamentByIdAsync(tournamentId);

        return CreatedAtAction(nameof(GetTournament), new { id = tournamentId }, tournament);
    }
}
