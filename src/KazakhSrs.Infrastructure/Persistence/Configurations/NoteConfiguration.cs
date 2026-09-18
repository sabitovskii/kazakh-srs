using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KazakhSrs.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("notes");

        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("id").UseIdentityByDefaultColumn();

        builder.Property(n => n.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(n => n.TopicId).HasColumnName("topic_id");
        builder.Property(n => n.Title).HasColumnName("title").IsRequired();
        builder.Property(n => n.Body).HasColumnName("body").IsRequired();
        builder.Property(n => n.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(n => n.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Topic>()
            .WithMany()
            .HasForeignKey(n => n.TopicId)
            .OnDelete(DeleteBehavior.SetNull);

        // Free-form, non-directional backlinks — mirrors the old note_links join table exactly,
        // including its composite PK and the "can't link to itself" check.
        builder.HasMany(n => n.LinkedNotes)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "note_links",
                right => right.HasOne<Note>().WithMany().HasForeignKey("linked_note_id")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Note>().WithMany().HasForeignKey("note_id").OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("note_id", "linked_note_id");
                    join.ToTable("note_links",
                        t => t.HasCheckConstraint("CK_note_links_no_self_link", "note_id <> linked_note_id"));
                });

        builder.Navigation(n => n.LinkedNotes)
            .HasField("_linkedNotes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}