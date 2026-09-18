using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KazakhSrs.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityByDefaultColumn();

        builder.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(c => c.TopicId).HasColumnName("topic_id");
        builder.Property(c => c.NoteId).HasColumnName("note_id");

        builder.Property(c => c.Type)
            .HasColumnName("type")
            .HasColumnType("card_type") // native Postgres enum — see KazakhSrsDbContext.HasPostgresEnum<CardType>()
            .IsRequired();

        builder.Property(c => c.PromptText).HasColumnName("prompt_text").IsRequired();
        builder.Property(c => c.CanonicalAnswer).HasColumnName("canonical_answer").IsRequired();
        builder.Property(c => c.ContextNotes).HasColumnName("context_notes");
        builder.Property(c => c.AudioUrl).HasColumnName("audio_url");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();

        // SM-2 state lives on the domain's own SpacedRepetitionState, mapped as an owned type
        // so the table shape matches the original schema while Card stays a thin wrapper around it.
        builder.OwnsOne(c => c.State, state =>
        {
            state.Property(s => s.Repetitions).HasColumnName("repetitions").HasDefaultValue(0).IsRequired();
            state.Property(s => s.IntervalDays).HasColumnName("interval_days").HasDefaultValue(0).IsRequired();
            state.Property(s => s.EaseFactor).HasColumnName("ease_factor").HasDefaultValue(2.5).IsRequired();
            state.Property(s => s.DueDate).HasColumnName("due_date").IsRequired();
        });

        builder.Navigation(c => c.State).IsRequired();

        // NB: composite index on the owned State.DueDate — double-check this resolves after
        // restore; owned-type index paths are the one thing here I couldn't compile-verify.
        builder.HasIndex(nameof(Card.UserId), "State.DueDate").HasDatabaseName("idx_cards_due");
        builder.HasIndex(c => c.TopicId).HasDatabaseName("idx_cards_topic");

        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Topic>().WithMany().HasForeignKey(c => c.TopicId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<Note>().WithMany().HasForeignKey(c => c.NoteId).OnDelete(DeleteBehavior.SetNull);
    }
}