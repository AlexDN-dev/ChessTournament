using API.DTOs;
using BLL.Dtos;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _service;

    public PlayerController(IPlayerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll()
    {
        var players = await _service.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerByUsername(string username)
    {
        var player = await _service.GetPlayerByUsernameAsync(username);
        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerRequest request)
    {
        var input = new CreatePlayerDto(
            request.Username,
            request.Email,
            request.Password,
            request.Birthday,
            request.Gender,
            request.Elo);

        var player = await _service.CreatePlayerAsync(input);

        return CreatedAtAction(
            nameof(GetPlayerByUsername),
            new { username = player.Username },
            player);
    }
}
