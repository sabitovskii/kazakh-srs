namespace KazakhSrs.Domain.Entities;

public class Note
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public long? TopicId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private readonly List<Note> _linkedNotes = new();

    /// <summary>Obsidian-style backlinks — free-form, no direction implied (mirrors the note_links table).</summary>
    public IReadOnlyCollection<Note> LinkedNotes => _linkedNotes.AsReadOnly();

    private Note() { } // EF Core

    public Note(long userId, string title, string body, DateTimeOffset createdAt, long? topicId = null)
    {
        UserId = userId;
        TopicId = topicId;
        Title = title;
        Body = body;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public void Update(string title, string body, DateTimeOffset updatedAt)
    {
        Title = title;
        Body = body;
        UpdatedAt = updatedAt;
    }

    public void LinkTo(Note other)
    {
        if (other.Id == Id)
            throw new InvalidOperationException("A note cannot link to itself.");

        if (!_linkedNotes.Contains(other))
            _linkedNotes.Add(other);
    }
}
