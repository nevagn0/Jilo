using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Discrip { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<AdverstmentCreate> AdverstmentCreates { get; set; } = new List<AdverstmentCreate>();

    public virtual ICollection<GamesUser> GamesUsers { get; set; } = new List<GamesUser>();
}
