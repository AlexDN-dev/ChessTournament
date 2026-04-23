using System.Security.Claims;
using System.Text;
using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;

namespace BLL.Service;

public class PlayerService : BaseService<Player, PlayerDto, CreatePlayerDto>, IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;

    public PlayerService(IPlayerRepository repository, IEmailService emailService, ITokenService tokenService)
        : base(repository)
    {
        _playerRepository = repository;
        _emailService = emailService;
        _tokenService = tokenService;
    }

    public async Task<PlayerDto> GetPlayerByUsernameAsync(string username)
    {
        var player = await _playerRepository.GetPlayerByUsernameAsync(username);
        if (player is null)
            throw new NotFoundException($"Aucun joueur trouvé avec le pseudo '{username}'.");

        return player.ToDto();
    }

    public override async Task<PlayerDto> CreateAsync(CreatePlayerDto dto)
    {
        if (!PlayerConstants.AllowedGenders.Contains(dto.Gender))
            throw new ValidationException($"Genre invalide. Valeurs autorisées : {string.Join(", ", PlayerConstants.AllowedGenders)}.");

        string password = CreatePassword(10);
        var entity = dto.ToEntity(password);
        entity.HashPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var created = await _repository.CreateAsync(entity);

        await _emailService.SendEmailAsync(dto.Email, "Création du compte ChessTournament",
            $@"<h1>Bonjour {entity.Username}, Bienvenue sur ChessTournament !</h1>
                <p>Voici le mot de passe de ton compte : {password}</p>
                <p>N'oublie pas de le changer après ta première connexion.</p>
                <p>Bon jeu à toi !</p>");

        return created!.ToDto();
    }

    public async Task<LoginDto> LoginPlayerAsync(LoginPlayerDto dto)
    {
        var player = await _playerRepository.GetPlayerByUsernameAsync(dto.Username);
        if (player is null)
            throw new ValidationException("Le nom d'utilisateur ou le mot de passe est incorrecte.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, player.HashPassword))
            throw new ValidationException("Le nom d'utilisateur ou le mot de passe est incorrecte.");

        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
            new Claim(ClaimTypes.Name, player.Username),
            new Claim(ClaimTypes.Role, player.Role.ToString())
        ];

        string token = _tokenService.GenerateAccessToken(claims);
        return player.toLoginDto(token);
    }

    protected override PlayerDto ToDto(Player entity) => entity.ToDto();

    protected override Player ToEntity(CreatePlayerDto dto)
        => throw new NotSupportedException("La création d'un joueur nécéssite un mot de passe.");

    private static string CreatePassword(int length)
    {
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890&@#{[^{}]";
        var res = new StringBuilder();
        var rnd = new Random();
        while (0 < length--)
            res.Append(valid[rnd.Next(valid.Length)]);
        return res.ToString();
    }
}
