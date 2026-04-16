using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Tournament
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int MinPlayer { get; set; }

    public int MaxPlayer { get; set; }

    public int? MinElo { get; set; }

    public int? MaxElo { get; set; }

    public string Status { get; set; } = null!;

    public int? ActualRound { get; set; }

    public bool WomenOnly { get; set; }

    public DateTime? FinalRegisterDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EncounterTournament> EncounterTournaments { get; set; } = new List<EncounterTournament>();

    public virtual ICollection<PlayerTournament> PlayerTournaments { get; set; } = new List<PlayerTournament>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
