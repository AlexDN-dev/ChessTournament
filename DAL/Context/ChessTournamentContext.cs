using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public partial class ChessTournamentContext : DbContext
{
    public ChessTournamentContext(DbContextOptions<ChessTournamentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<EncounterTournament> EncounterTournaments { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerTournament> PlayerTournaments { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC073141A890");

            entity.HasIndex(e => e.Name, "UQ__Categori__737584F62266D9BE").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EncounterTournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Encounte__3214EC075E8B4596");

            entity.HasIndex(e => new { e.TournamentId, e.Player1, e.Player2, e.Round }, "IX_match_unique").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.EncounterDate).HasColumnType("datetime");
            entity.Property(e => e.Result)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Player1Navigation).WithMany(p => p.EncounterTournamentPlayer1Navigations)
                .HasForeignKey(d => d.Player1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Encounter__Playe__412EB0B6");

            entity.HasOne(d => d.Player2Navigation).WithMany(p => p.EncounterTournamentPlayer2Navigations)
                .HasForeignKey(d => d.Player2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Encounter__Playe__4222D4EF");

            entity.HasOne(d => d.Tournament).WithMany(p => p.EncounterTournaments)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK__Encounter__Tourn__403A8C7D");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Players__3214EC07A99FB9A6");

            entity.HasIndex(e => e.Username, "UQ__Players__536C85E4CF5A1D31").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Players__A9D1053478F7EBB3").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Elo).HasDefaultValue(1200);
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.HashPassword)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasDefaultValue(0);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlayerTournament>(entity =>
        {
            entity.HasKey(e => new { e.TournamentId, e.PlayerId }).HasName("PK__PlayerTo__38C7F45F720E52DA");

            entity.Property(e => e.RegisterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerTournaments)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK__PlayerTou__Playe__3C69FB99");

            entity.HasOne(d => d.Tournament).WithMany(p => p.PlayerTournaments)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK__PlayerTou__Tourn__3D5E1FD2");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC079500E564");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ActualRound).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FinalRegisterDate).HasColumnType("datetime");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasMany(d => d.Categories).WithMany(p => p.Tournaments)
                .UsingEntity<Dictionary<string, object>>(
                    "TournamentCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK__Tournamen__Categ__3F466844"),
                    l => l.HasOne<Tournament>().WithMany()
                        .HasForeignKey("TournamentId")
                        .HasConstraintName("FK__Tournamen__Tourn__3E52440B"),
                    j =>
                    {
                        j.HasKey("TournamentId", "CategoryId").HasName("PK__Tourname__0DF380B310E09354");
                        j.ToTable("TournamentCategories");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
