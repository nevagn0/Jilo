using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Jilo.Models;

public partial class JiloContext : DbContext
{
    public JiloContext()
    {
    }

    public JiloContext(DbContextOptions<JiloContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdversmetUser> AdversmetUsers { get; set; }

    public virtual DbSet<Comm> Comms { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GamesUser> GamesUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Jilo;Username=postgres;Password=Misha1029!");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdversmetUser>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdGame });
            entity.ToTable("adversmet_user");

            entity.HasIndex(e => new { e.IdUser, e.IdGame }, "adversmet_user_unique").IsUnique();

            entity.Property(e => e.DateCreate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_create");
            entity.Property(e => e.Discription)
                .HasColumnType("character varying")
                .HasColumnName("discription");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.IdSecondUser).HasColumnName("id_second_user");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.NameSecondUser)
                .HasColumnType("character varying")
                .HasColumnName("name_second_user");

            entity.HasOne(d => d.IdGameNavigation).WithMany()
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adversmet_user_games_fk");

            entity.HasOne(d => d.IdUserNavigation).WithMany()
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adversmet_user_user_fk");
        });

        modelBuilder.Entity<Comm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comm_pk");

            entity.ToTable("comm");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Comm1)
                .HasColumnType("character varying")
                .HasColumnName("comm");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Targetuser).HasColumnName("targetuser");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comms)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("comm_user_fk");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pk");

            entity.ToTable("games");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.Discrip)
                .HasColumnType("character varying")
                .HasColumnName("discrip");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<GamesUser>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdGame }).HasName("games_user_pk");

            entity.ToTable("games_user");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Rank)
                .HasColumnType("character varying")
                .HasColumnName("rank");
            entity.Property(e => e.Role)
                .HasColumnType("character varying")
                .HasColumnName("role");
            entity.Property(e => e.TimeInGame)
                .HasColumnType("character varying")
                .HasColumnName("time_in_game");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.GamesUsers)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("games_user_games_fk");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.GamesUsers)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("games_user_user_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.Comm)
                .HasColumnType("character varying")
                .HasColumnName("comm");
            entity.Property(e => e.DataRegistration).HasColumnName("data_registration");
            entity.Property(e => e.Discription)
                .HasColumnType("character varying")
                .HasColumnName("discription");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.LastOnline)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_online");
            entity.Property(e => e.Passwordhash)
                .HasColumnType("character varying")
                .HasColumnName("passwordhash");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'\"USER\"'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.Socialcredits).HasColumnName("socialcredits");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
