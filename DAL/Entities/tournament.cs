using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class tournament
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public string location { get; set; } = null!;

    public int minPlayer { get; set; }

    public int maxPlayer { get; set; }

    public int? minElo { get; set; }

    public int? maxElo { get; set; }

    public string status { get; set; } = null!;

    public int? actualRound { get; set; }

    public bool womenOnly { get; set; }

    public DateTime? finalRegisterDate { get; set; }

    public DateTime? createdAt { get; set; }

    public DateTime? updatedAt { get; set; }

    public virtual ICollection<encounterTournament> encounterTournaments { get; set; } = new List<encounterTournament>();

    public virtual ICollection<playerTournament> playerTournaments { get; set; } = new List<playerTournament>();

    public virtual ICollection<category> categories { get; set; } = new List<category>();
}
