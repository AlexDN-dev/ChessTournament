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

        var tournament = await _repository.GetByIdAsync(id);
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
        Tournament? tournament = await _repository.GetByIdAsync(id);
        if (tournament is null)
            throw new NotFoundException("Le tournoi est introuvable");
        if (tournament.Status != TournamentStatus.PENDING)
            throw new DeletedRowInaccessibleException(
                "Seuls les tournoi qui n'ont pas encore commencé peuvent être supprimé");
        
        await _repository.DeleteAsync(id);
    }

    public async Task RegisterPlayerToTournamentAsync(string playerUsername, Guid tournamentId)
    {
        Tournament? tournament = await _repository.GetByIdAsync(tournamentId);
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
        Tournament? tournament = await _repository.GetByIdAsync(tournamentId);
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

    public async Task StartTournamentAsync(Guid tournamentId)
    {
        Tournament? tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException("Impossible de trouver le tournoi.");
        if (tournament.FinalRegisterDate > DateTime.Now)
            throw new ValidationException("La date d'inscription n'est pas encoré arrivé a son terme.");
        if (tournament.MinPlayer > tournament.PlayerTournaments.Count)
            throw new InvalidDataException("Impossible de démarrer le tournoi car il manque des joueurs");

        List<EncounterTournament> encounterTournaments = GenerateRoundRobin(tournament.PlayerTournaments.Select(p => p.PlayerId).ToList(), tournamentId);
        await _repository.StartTournament(encounterTournaments, tournamentId);
    }

    // Command pur : valide et enregistre le résultat, sans effet de bord supplémentaire
    public async Task UpdateEncounterAsync(Guid encounterId, string result)
    {
        var encounter = await _repository.GetEncounterByIdAsync(encounterId);
        if (encounter is null)
            throw new NotFoundException("Rencontre introuvable.");

        var tournament = await _repository.GetByIdAsync(encounter.TournamentId);
        if (tournament is null)
            throw new NotFoundException("Tournoi introuvable.");

        if (encounter.Round != tournament.ActualRound)
            throw new ValidationException("Seule une rencontre de la round courante peut être modifiée.");

        await _repository.UpdateEncounterAsync(encounterId, result);
    }

    // Query pure : recharge depuis la DB (données à jour après l'update)
    public async Task<bool> IsRoundCompleteAsync(Guid tournamentId)
    {
        var tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException("Tournoi introuvable.");

        return tournament.EncounterTournaments
            .Where(e => e.Round == tournament.ActualRound)
            .All(e => e.Result != null);
    }

    // Command : termine le tournoi uniquement si c'est bien la dernière ronde
    public async Task FinishTournamentIfLastRoundAsync(Guid tournamentId)
    {
        var tournament = await _repository.GetByIdAsync(tournamentId);
        if (tournament is null)
            throw new NotFoundException("Tournoi introuvable.");

        int totalRounds = (tournament.PlayerTournaments.Count - 1) * 2;
        if (tournament.ActualRound >= totalRounds)
            await _repository.FinishTournamentAsync(tournamentId);
    }

    public async Task NextRoundAsync(Guid tournamentId)
    {
        Tournament? tournament = await _repository.GetByIdAsync(tournamentId);
        int totalRounds = (tournament.PlayerTournaments.Count - 1) * 2;
        
        if (tournament is null)
            throw new NotFoundException("Impossible de trouver le tournoi.");
        if (tournament.EncounterTournaments.Any(et => et.Result == null))
            throw new ValidationException(
                "Impossible de passer à la round suivante, il manque un/des résultats aux matchs actuels");
        if (tournament.Status != TournamentStatus.STARTED)
            throw new ValidationException("Le tournoi n'a pas encore commencé ou est terminé.");
        if (tournament.ActualRound >= totalRounds)
            throw new ValidationException("Le tournoi est déjà à sa dernière round.");

        await _repository.NextRoundAsync(tournamentId);
    }

    private static List<EncounterTournament> GenerateRoundRobin(List<Guid> playerIds, Guid tournamentId)
    {
        var players = new List<Guid>(playerIds);

        if (players.Count % 2 != 0)
            players.Add(Guid.Empty);

        int n = players.Count;
        int maxRound = n - 1;
        int matchesPerRound = n / 2;
        var encounters = new List<EncounterTournament>();

        for (int count = 0; count < 2; count++)
        {
            bool isReturn = count == 1;

            for (int round = 0; round < maxRound; round++)
            {
                int roundNumber = round + (count * maxRound) + 1;

                for (int i = 0; i < matchesPerRound; i++)
                {
                    Guid white = players[i];
                    Guid black = players[n - 1 - i];

                    if (white == Guid.Empty || black == Guid.Empty)
                        continue;

                    encounters.Add(new EncounterTournament
                    {
                        TournamentId = tournamentId,
                        Player1      = isReturn ? black : white,
                        Player2      = isReturn ? white : black,
                        Round        = roundNumber,
                        Result       = null,
                        EncounterDate = DateTime.Today.AddDays(roundNumber - 1)
                    });
                }
                
                players.Insert(1, players[players.Count - 1]);
                players.RemoveAt(players.Count - 1);
            }
        }

        return encounters;
    }
}
