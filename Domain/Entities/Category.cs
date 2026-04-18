using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
