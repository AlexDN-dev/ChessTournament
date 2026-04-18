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

    public TournamentService(ITournamentRepository repository)
    {
        _repository = repository;
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
}
