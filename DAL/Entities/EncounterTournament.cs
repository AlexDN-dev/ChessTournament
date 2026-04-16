using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class EncounterTournament
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid Player1 { get; set; }

    public Guid Player2 { get; set; }

    public string? Result { get; set; }

    public int Round { get; set; }

    public DateTime EncounterDate { get; set; }

    public virtual Player Player1Navigation { get; set; } = null!;

    public virtual Player Player2Navigation { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
