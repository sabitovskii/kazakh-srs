using KazakhSrs.Domain.Entities;
using KazakhSrs.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KazakhSrs.Infrastructure.Persistence;

public class KazakhSrsDbContext : DbContext
{
    public KazakhSrsDbContext(DbContextOptions<KazakhSrsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<ReviewLog> ReviewLogs => Set<ReviewLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Native Postgres enum, mirrors db/migrations' old `card_type` — see CardConfiguration
        // for the column mapping (HasColumnType("card_type")).
        modelBuilder.HasPostgresEnum<CardType>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KazakhSrsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
