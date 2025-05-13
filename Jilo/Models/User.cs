using System;
using System.Collections.Generic;

namespace Jilo.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public int Id { get; set; }

    public double? Socialcredits { get; set; }

    public DateOnly DataRegistration { get; set; }

    public string? Avatar { get; set; }

    public DateTime? LastOnline { get; set; }

    public string? Comm { get; set; }

    public double? Grade { get; set; }

    public string? Role { get; set; }

    public string? Discription { get; set; }

    public virtual ICollection<Comm> Comms { get; set; } = new List<Comm>();

    public virtual ICollection<GamesUser> GamesUsers { get; set; } = new List<GamesUser>();
}
