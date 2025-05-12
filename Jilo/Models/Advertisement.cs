using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class Advertisement
{
    public int Id { get; set; }

    public int? IdUser { get; set; }

    public int? IdTim { get; set; }

    public int? Games { get; set; }

    public string? Discription { get; set; }

    public DateTime? DateCreate { get; set; }

    public virtual ICollection<AdverstmentCreate> AdverstmentCreates { get; set; } = new List<AdverstmentCreate>();

    public virtual User? IdUserNavigation { get; set; }
}
