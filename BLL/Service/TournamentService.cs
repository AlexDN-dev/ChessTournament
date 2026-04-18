using System.Data;
using BLL.Dtos;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Filters;

namespace BLL.Service;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _repository;
    private readonly IPlayerRepository _playerRepository;

    public TournamentService(ITournamentRepository repository, IPlayerRepository playerRepository)
    {
        _repository = repository;
        _playerRepository = playerRepository;
    }

    public async Task<IEnumerable<TournamentSummaryDto>> GetAllAsync(TournamentFilter filter)
    {
        var tournaments = await _repository.GetAllAsync(filter);
        return tournaments.Select(t => t.ToSummaryDto());
    }

    public async Task<TournamentDetailDto> GetTournamentByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("L'identifiant du tournoi est invalide.");

        var tournament = await _repository.GetTournamentByIdAsync(id);
        if (tournament is null)
            throw new NotFoundException($"Aucun tournoi trouvé avec l'identifiant '{id}'.");

        return tournament.ToDetailDto();
    }

    public async Task<Guid> CreateTournamentAsync(CreateTournamentDto dto)
    {
        if (dto.MinPlayer <= 3)
            throw new ValidationException("Il faut un minimum de 4 joueurs pour un tournoi.");
        if (dto.MinPlayer > dto.MaxPlayer)
            throw new ValidationException("Le nombre minimal de joueur est supérieur au nombre maximum.");
        if (dto.FinalRegisterDate <= DateTime.Now)
            throw new ValidationException("La date de final des inscription est déjà passée.");
        if (dto.CategoryIds.Count == 0 || dto.CategoryIds.Count > 2)
            throw new ValidationException("Il faut au minimum 1 catégorie ou au maximum 2.");
        if (dto.MinElo is not null && dto.MaxElo is not null)
            if (dto.MinElo > dto.MaxElo)
                throw new ValidationException("L'elo minimum est supérieur à l'élo maximum.");

        Tournament t = new Tournament
        {
            Name = dto.Name,
            Location = dto.Location,
            MinPlayer = dto.MinPlayer,
            MaxPlayer = dto.MaxPlayer,
            MinElo = dto.MinElo,
            MaxElo = dto.MaxElo,
            Status = TournamentStatus.PENDING,
            ActualRound = 0,
            WomenOnly = dto.WomenOnly,
            FinalRegisterDate = dto.FinalRegisterDate,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        Guid tournamentId =  _repository.CreateTournamentAsync(t, dto.CategoryIds);
        return tournamentId;
    }

    public async Task DeleteTournamentAsync(Guid id)
    {
        Tournament? tournament = await _repository.GetTournamentByIdAsync(id);
        if (tournament is null)
            throw new NotFoundException("Le tournoi est introuvable");
        if (tournament.Status != TournamentStatus.PENDING)
            throw new DeletedRowInaccessibleException(
                "Seuls les tournoi qui n'ont pas encore commencé peuvent être supprimé");
        
        await _repository.DeleteTournamentAsync(id);
    }

    public async Task RegisterPlayerToTournamentAsync(string playerUsername, Guid tournamentId)
    {
        Tournament? tournament = await _repository.GetTournamentByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException("Impossible de trouver le tournoi.");
        Player? player = await _playerRepository.GetPlayerByUsernameAsync(playerUsername);
        if (player is null)
            throw new NotFoundException("Impossible de trouver le joueur");

        if (tournament.Status != TournamentStatus.PENDING)
            throw new ValidationException("Impossible d'inscrire le joueur, le tournoir a déjà commencé.");
        if (tournament.FinalRegisterDate <= DateTime.Now)
            throw new ExpirationRegisterDateException("La date d'inscription est dépassée");
        if (tournament.PlayerTournaments.Any(p => p.PlayerId == player.Id))
            throw new PlayerAlreadyRegisterException("Le joueur est déjà inscrit dans ce tournoi");
        if (tournament.MaxPlayer == tournament.PlayerTournaments.Count)
            throw new InvalidDataException("Le maximum de joueur a été atteint pour ce tournoi.");
        
        int age = tournament.FinalRegisterDate.HasValue
            ? (int)((tournament.FinalRegisterDate.Value - player.Birthday).TotalDays / 365.25)
            : throw new ValidationException("Le tournoi n'a pas de date de fin d'inscription.");
        
        if (!tournament.Categories.Any(c => age >= c.MinAge && age <= c.MaxAge))
            throw new ValidationException("L'âge du joueur ne correspond à aucune catégorie autorisée.");

        if (tournament.MinElo is not null && player.Elo < tournament.MinElo)
            throw new ValidationException("Le joueur n'a pas assez d'ELO pour rejoindre ce tournoi");
        if (tournament.MaxElo is not null && player.Elo > tournament.MaxElo)
            throw new ValidationException("Le joueur a trop d'ELO pour rejoindre ce tournoi");
        if (tournament.WomenOnly && player.Gender != "Femme")
            throw new ValidationException("Le joueur doit être une femme pour participer à ce tournoi.");

        PlayerTournament pt = new PlayerTournament
            { TournamentId = tournament.Id, PlayerId = player.Id, RegisterDate = DateTime.Now };

        await _repository.RegisterPlayerToTournamentAsync(pt);
    }

    public async Task UnsubscribePlayerFromTournamentAsync(string playerUsername, Guid tournamentId)
    {
        Tournament? tournament = await _repository.GetTournamentByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException("Impossible de trouver le tournoi.");
        Player? player = await _playerRepository.GetPlayerByUsernameAsync(playerUsername);
        if (player is null)
            throw new NotFoundException("Impossible de trouver le joueur");

        if (tournament.Status != TournamentStatus.PENDING)
            throw new ValidationException("Impossible de désinscrire le joueur car le tournoi à déjà commencé.");
        if (tournament.PlayerTournaments.All(p => p.PlayerId != player.Id))
            throw new ValidationException("Le joueur n'est pas inscrit à ce tournoi.");
        PlayerTournament pt = new PlayerTournament
            { TournamentId = tournament.Id, PlayerId = player.Id };
        await _repository.UnsubscribePlayerFromTournamentAsync(pt);

    }
}
