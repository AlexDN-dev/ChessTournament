using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class player
{
    public Guid id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string hashPassword { get; set; } = null!;

    public DateTime birthday { get; set; }

    public string gender { get; set; } = null!;

    public int? role { get; set; }

    public int? elo { get; set; }

    public virtual ICollection<encounterTournament> encounterTournamentplayer1Navigations { get; set; } = new List<encounterTournament>();

    public virtual ICollection<encounterTournament> encounterTournamentplayer2Navigations { get; set; } = new List<encounterTournament>();

    public virtual ICollection<playerTournament> playerTournaments { get; set; } = new List<playerTournament>();
}
