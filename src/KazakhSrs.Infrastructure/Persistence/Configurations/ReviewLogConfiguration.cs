using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KazakhSrs.Infrastructure.Persistence.Configurations;

public class ReviewLogConfiguration : IEntityTypeConfiguration<ReviewLog>
{
    public void Configure(EntityTypeBuilder<ReviewLog> builder)
    {
        builder.ToTable("review_log", t => t.HasCheckConstraint("CK_review_log_grade_range", "grade BETWEEN 0 AND 5"));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").UseIdentityByDefaultColumn();

        builder.Property(r => r.CardId).HasColumnName("card_id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(r => r.ReviewedAt).HasColumnName("reviewed_at").IsRequired();
        builder.Property(r => r.Grade).HasColumnName("grade").HasColumnType("smallint").IsRequired();
        builder.Property(r => r.UserResponseText).HasColumnName("user_response_text");
        builder.Property(r => r.RawAudioRef).HasColumnName("raw_audio_ref");

        builder.HasIndex(r => new { r.CardId, r.ReviewedAt }).HasDatabaseName("idx_review_log_card");

        builder.HasOne<Card>().WithMany().HasForeignKey(r => r.CardId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}