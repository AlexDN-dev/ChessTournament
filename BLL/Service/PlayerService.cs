using System.Text.RegularExpressions;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace BLL.Service;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;

    public PlayerService(IPlayerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        var players = await _repository.GetAllAsync();
        return players;
    }

    public async Task<Player> GetPlayerByUsernameAsync(string username)
    {
        var player = await _repository.GetPlayerByUsernameAsync(username);
        if (player is null)
        {
            throw new KeyNotFoundException("Aucun joueur trouvé avec cet email.");
        }

        return player;
    }

    public async Task<Player> CreatePlayerAsync(Player player)
    {
        var isExisting = await _repository.IfPlayerExistAsync(player.Email, player.Username);
        if (isExisting != null)
        {
            if (isExisting.Username == player.Username)
                throw new Exception("Username déjà utilisé");

            if (isExisting.Email == player.Email)
                throw new Exception("Email déjà utilisé");
        }

        if (player.Gender != "Homme" && player.Gender != "Femme")
        {
            throw new Exception("Merci de rentrer un genre valide");
        }

        
        var regex = new Regex(@"^(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$");
        bool isValidPassword = regex.IsMatch(player.HashPassword);
        if (!isValidPassword)
        {
            throw new Exception("Le mot de passe doit avoir au moins 8 caractères, un chiffre et un caractère spécial");
        }
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(player.HashPassword);
        player.HashPassword = hashedPassword;
        var p = await _repository.CreatePlayerAsync(player);
        return p;
    }
}