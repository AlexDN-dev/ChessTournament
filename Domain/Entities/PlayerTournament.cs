using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class PlayerTournament
{
    public Guid TournamentId { get; set; }

    public Guid PlayerId { get; set; }

    public DateTime? RegisterDate { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
