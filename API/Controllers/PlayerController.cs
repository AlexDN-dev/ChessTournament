using API.DTOs;
using BLL.Dtos;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            request.Birthday,
            request.Gender,
            request.Elo);

        // Command : retourne seulement l'Id
        var id = await _service.CreateAsync(input);
        // Query : récupère le DTO complet pour la réponse 201
        var player = await _service.GetByIdAsync(id);

        return CreatedAtAction(
            nameof(GetPlayerByUsername),
            new { username = player.Username },
            player);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginDto>> LoginPlayer(LoginPlayerRequest request)
    {
        var input = new LoginPlayerDto(
            request.Username,
            request.Password
        );

        var player = await _service.LoginPlayerAsync(input);

        return Ok(player);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult GetAdminData()
    {
        return Ok("Vous êtes bien admin, bienvenue");
    }
}
