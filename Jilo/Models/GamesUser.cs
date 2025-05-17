using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jilo.Models;

public partial class GamesUser
{
    public int IdUser { get; set; }

    public int IdGame { get; set; }

    public string? Rank { get; set; }

    public string? TimeInGame { get; set; }

    public string? Role { get; set; }
    [JsonIgnore]
    public virtual Game IdGameNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual User IdUserNavigation { get; set; } = null!;
}
