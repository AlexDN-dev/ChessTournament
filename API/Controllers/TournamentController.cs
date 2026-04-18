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

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTournament(Guid id)
    {
        await _service.DeleteTournamentAsync(id);
        return NoContent();
    }

    [HttpPost("register-player")]
    public async Task<ActionResult> RegisterPlayerToTournament(string playerUsername, Guid tournamentId)
    {
        await _service.RegisterPlayerToTournamentAsync(playerUsername, tournamentId);
        return Ok("Le joueur a bien été inscrit au tournoi.");
    }

    [HttpDelete("unsubscribe-player")]
    public async Task<ActionResult> UnsubscribePlayerFromTournament(string playerUsername, Guid tournamentId)
    {
        await _service.UnsubscribePlayerFromTournamentAsync(playerUsername, tournamentId);
        return Ok("Le joueur à bien été désinscrit du tournoi.");
    }

    [HttpPut("start-tournament")]
    public async Task<ActionResult> StartTournament(Guid tournamentId)
    {
        await _service.StartTournamentAsync(tournamentId);
        return Ok("Le tournoi a bien débuté.");
    }

    [HttpPut("update-encounter")]
    public async Task<ActionResult> UpdateEncounter(Guid encounterId, string result)
    {
        await _service.UpdateEncounterAsync(encounterId, result);
        return Ok("Le resultat de la rencontre a bien été mis à jour.");
    }
}
