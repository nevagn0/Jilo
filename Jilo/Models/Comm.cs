using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class Comm
{
    public int Id { get; set; }

    public int? IdUser { get; set; }

    public string? Comm1 { get; set; }

    public double? Grade { get; set; }

    public int? Targetuser { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
