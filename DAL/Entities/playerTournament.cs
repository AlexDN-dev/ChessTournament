using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class playerTournament
{
    public Guid tournamentId { get; set; }

    public Guid playerId { get; set; }

    public DateTime? registerDate { get; set; }

    public virtual player player { get; set; } = null!;

    public virtual tournament tournament { get; set; } = null!;
}
