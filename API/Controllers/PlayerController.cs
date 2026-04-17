using API.DTOs;
using BLL.Interfaces;
using Domain.Entities;
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
    // GET api/player
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var players = await _service.GetAllAsync();
        return Ok(players.Select(ToPlayerDto));
    }
    //GET api/player/{username}
    [HttpGet("{username}")]
    public async Task<IActionResult> GetPlayerByUsername(string username)
    {
        var player = await _service.GetPlayerByUsernameAsync(username);
        return Ok(player);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayer(CreatePlayerDto createPlayerDto)
    {
        var player = await _service.CreatePlayerAsync(ToEntity(createPlayerDto));

        return CreatedAtAction(
            nameof(GetPlayerByUsername),
            new { username = player.Username },
            ToPlayerDto(player)
        );
    }
    
    private static PlayerDto ToPlayerDto(Player p)
        => new(p.Id, p.Username,p.Email, p.Birthday, p.Gender, p.Elo);

    private static Player ToEntity(CreatePlayerDto p)
    {
        return new Player
        {
            Username = p.Username,
            Email = p.Email,
            HashPassword = p.Password,
            Birthday = p.BirthDay,
            Gender = p.Gender
        };
    }
}