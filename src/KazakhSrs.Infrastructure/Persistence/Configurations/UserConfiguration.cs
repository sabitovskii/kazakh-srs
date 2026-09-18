using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KazakhSrs.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").UseIdentityByDefaultColumn();

        builder.Property(u => u.TelegramId).HasColumnName("telegram_id").IsRequired();
        builder.HasIndex(u => u.TelegramId).IsUnique();

        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
    }
}