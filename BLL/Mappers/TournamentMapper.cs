using BLL.Dtos;
using Domain.Entities;

namespace BLL.Mappers;

internal static class TournamentMapper
{
    public static TournamentSummaryDto ToSummaryDto(this Tournament t)
        => new(
            t.Id,
            t.Name,
            t.Location,
            t.MinPlayer,
            t.MaxPlayer,
            t.MinElo,
            t.MaxElo,
            t.Status,
            t.ActualRound,
            t.WomenOnly,
            t.FinalRegisterDate,
            t.CreatedAt,
            t.UpdatedAt,
            t.Categories.Select(c => c.ToDto()),
            t.PlayerTournaments.Count);

    public static TournamentDetailDto ToDetailDto(this Tournament t)
        => new(
            t.Id,
            t.Name,
            t.Location,
            t.MinPlayer,
            t.MaxPlayer,
            t.MinElo,
            t.MaxElo,
            t.Status,
            t.ActualRound,
            t.WomenOnly,
            t.FinalRegisterDate,
            t.CreatedAt,
            t.UpdatedAt,
            t.Categories.Select(c => c.ToDto()),
            t.PlayerTournaments.Select(pt => pt.Player.ToSummaryDto()),
            t.EncounterTournaments
                .Where(e => e.Round == t.ActualRound)
                .Select(e => new EncounterDto(
                    e.Id,
                    e.Player1Navigation.Username,
                    e.Player2Navigation.Username,
                    e.Result,
                    e.EncounterDate)));
}
