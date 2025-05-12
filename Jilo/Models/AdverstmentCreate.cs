using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class AdverstmentCreate
{
    public int Id { get; set; }

    public int? IdAdversment { get; set; }

    public int? IdGame { get; set; }

    public virtual Advertisement? IdAdversmentNavigation { get; set; }

    public virtual Game? IdGameNavigation { get; set; }
}
