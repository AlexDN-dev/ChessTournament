using BLL.Dtos;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly IScoreService _service;

    public ScoreController(IScoreService service)
    {
        _service = service;
    }

    [HttpGet("{tournamentId:guid}")]
    public async Task<ActionResult<IEnumerable<PlayerScoreDto>>> GetScoreTable(
        Guid tournamentId,
        [FromQuery] int? round)
    {
        var scores = await _service.GetScoreTableAsync(tournamentId, round);
        return Ok(scores);
    }
}
