using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Constants;
using Domain.Exceptions;

namespace BLL.Service;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;

    public PlayerService(IPlayerRepository repository, IEmailService emailService, ITokenService tokenservice)
    {
        _repository = repository;
        _emailService = emailService;
        _tokenService = tokenservice;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        var players = await _repository.GetAllAsync();
        return players.Select(p => p.ToDto());
    }

    public async Task<PlayerDto> GetPlayerByUsernameAsync(string username)
    {
        var player = await _repository.GetPlayerByUsernameAsync(username);
        if (player is null)
            throw new NotFoundException($"Aucun joueur trouvé avec le pseudo '{username}'.");

        return player.ToDto();
    }

    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
    {
        if (!PlayerConstants.AllowedGenders.Contains(dto.Gender))
            throw new ValidationException($"Genre invalide. Valeurs autorisées : {string.Join(", ", PlayerConstants.AllowedGenders)}.");
        
        string password = CreatePassword(10);
        var entity = dto.ToEntity(password);
        entity.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var created = await _repository.CreatePlayerAsync(entity);

        await _emailService.SendEmailAsync(dto.Email, "Création du compte ChessTournament",
            $@"<h1>Bonjour {entity.Username}, Bienvenue sur ChessTournament !</h1>
                <p>Voici le mot de passe de ton compte : {password}</p>
                <p>N'oublie pas de le changer après ta première connexion.</p>
                <p>Bon jeu à toi !</p>");
        
        return created!.ToDto();
    }

    public async Task<LoginDto> LoginPlayerAsync(LoginPlayerDto dto)
    {
        var player = await _repository.GetPlayerByUsernameAsync(dto.Username);
        if (player is null)
            throw new ValidationException("Le nom d'utilisateur ou le mot de passe est incorrecte.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, player.HashPassword))
            throw new ValidationException("Le nom d'utilisateur ou le mot de passe est incorrecte.");

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new Claim(ClaimTypes.Name, player.Username),
            new Claim(ClaimTypes.Role, player.Role.ToString())
        };

        string token = _tokenService.GenerateAccessToken(claims);
        return player.toLoginDto(token);
    }

    private static string CreatePassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890&@#{[^{}]";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }
}
