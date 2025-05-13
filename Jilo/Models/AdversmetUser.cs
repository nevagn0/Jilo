using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class AdversmetUser
{
    public int IdUser { get; set; }

    public int IdGame { get; set; }

    public string? Discription { get; set; }

    public DateTime? DateCreate { get; set; }

    public int? IdSecondUser { get; set; }

    public string? NameSecondUser { get; set; }

    public virtual Game IdGameNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
