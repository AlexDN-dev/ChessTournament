using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class category
{
    public Guid id { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<tournament> tournaments { get; set; } = new List<tournament>();
}
