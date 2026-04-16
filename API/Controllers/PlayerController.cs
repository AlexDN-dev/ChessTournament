using BLL.DTOs;
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
    // GET api/player
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var players = await _service.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetPlayerByUsername(string username)
    {
        var player = await _service.GetPlayerByUsernameAsync(username);
        return Ok(player);
    }
}