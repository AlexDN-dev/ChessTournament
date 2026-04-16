using System;
using System.Collections.Generic;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public partial class ChessTournamentContext : DbContext
{
    public ChessTournamentContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<category> categories { get; set; }

    public virtual DbSet<encounterTournament> encounterTournaments { get; set; }

    public virtual DbSet<player> players { get; set; }

    public virtual DbSet<playerTournament> playerTournaments { get; set; }

    public virtual DbSet<tournament> tournaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__categori__3213E83FDD962333");

            entity.HasIndex(e => e.name, "UQ__categori__72E12F1B70083127").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<encounterTournament>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__encounte__3213E83FFC60F125");

            entity.HasIndex(e => new { e.tournamentId, e.player1, e.player2, e.round }, "IX_match_unique").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.encounterDate).HasColumnType("datetime");
            entity.Property(e => e.result)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.player1Navigation).WithMany(p => p.encounterTournamentplayer1Navigations)
                .HasForeignKey(d => d.player1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__encounter__playe__412EB0B6");

            entity.HasOne(d => d.player2Navigation).WithMany(p => p.encounterTournamentplayer2Navigations)
                .HasForeignKey(d => d.player2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__encounter__playe__4222D4EF");

            entity.HasOne(d => d.tournament).WithMany(p => p.encounterTournaments)
                .HasForeignKey(d => d.tournamentId)
                .HasConstraintName("FK__encounter__tourn__403A8C7D");
        });

        modelBuilder.Entity<player>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__players__3213E83F70336CA4");

            entity.HasIndex(e => e.email, "UQ__players__AB6E616443A93C28").IsUnique();

            entity.HasIndex(e => e.username, "UQ__players__F3DBC57232DCFE23").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.birthday).HasColumnType("datetime");
            entity.Property(e => e.elo).HasDefaultValue(1200);
            entity.Property(e => e.email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.hashPassword)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.role).HasDefaultValue(0);
            entity.Property(e => e.username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<playerTournament>(entity =>
        {
            entity.HasKey(e => new { e.tournamentId, e.playerId }).HasName("PK__playerTo__903644D7082EBEDF");

            entity.Property(e => e.registerDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.player).WithMany(p => p.playerTournaments)
                .HasForeignKey(d => d.playerId)
                .HasConstraintName("FK__playerTou__playe__3C69FB99");

            entity.HasOne(d => d.tournament).WithMany(p => p.playerTournaments)
                .HasForeignKey(d => d.tournamentId)
                .HasConstraintName("FK__playerTou__tourn__3D5E1FD2");
        });

        modelBuilder.Entity<tournament>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tourname__3213E83F63CF1C1E");

            entity.Property(e => e.id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.actualRound).HasDefaultValue(0);
            entity.Property(e => e.createdAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.finalRegisterDate).HasColumnType("datetime");
            entity.Property(e => e.location)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");
            entity.Property(e => e.updatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasMany(d => d.categories).WithMany(p => p.tournaments)
                .UsingEntity<Dictionary<string, object>>(
                    "tournamentCategory",
                    r => r.HasOne<category>().WithMany()
                        .HasForeignKey("categoryId")
                        .HasConstraintName("FK__tournamen__categ__3F466844"),
                    l => l.HasOne<tournament>().WithMany()
                        .HasForeignKey("tournamentId")
                        .HasConstraintName("FK__tournamen__tourn__3E52440B"),
                    j =>
                    {
                        j.HasKey("tournamentId", "categoryId").HasName("PK__tourname__00C74BD560BC92B1");
                        j.ToTable("tournamentCategories");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
