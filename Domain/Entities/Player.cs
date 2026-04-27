using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Player : IEntity
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashPassword { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string Gender { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? Elo { get; set; }

    public virtual ICollection<EncounterTournament> EncounterTournamentPlayer1Navigations { get; set; } = new List<EncounterTournament>();

    public virtual ICollection<EncounterTournament> EncounterTournamentPlayer2Navigations { get; set; } = new List<EncounterTournament>();

    public virtual ICollection<PlayerTournament> PlayerTournaments { get; set; } = new List<PlayerTournament>();
}
