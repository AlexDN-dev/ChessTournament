using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class encounterTournament
{
    public Guid id { get; set; }

    public Guid tournamentId { get; set; }

    public Guid player1 { get; set; }

    public Guid player2 { get; set; }

    public string? result { get; set; }

    public int round { get; set; }

    public DateTime encounterDate { get; set; }

    public virtual player player1Navigation { get; set; } = null!;

    public virtual player player2Navigation { get; set; } = null!;

    public virtual tournament tournament { get; set; } = null!;
}
