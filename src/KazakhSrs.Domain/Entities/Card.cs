using KazakhSrs.Domain.Enums;

namespace KazakhSrs.Domain.Entities;

public class Card
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public long? TopicId { get; private set; }
    public long? NoteId { get; private set; }
    public CardType Type { get; private set; }

    public string PromptText { get; private set; } = null!;
    public string CanonicalAnswer { get; private set; } = null!;
    public string? ContextNotes { get; private set; }
    public string? AudioUrl { get; private set; }

    /// <summary>SM-2 scheduling state — see <see cref="Sm2Engine"/>.</summary>
    public SpacedRepetitionState State { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    private Card() { } // EF Core

    public Card(
        long userId,
        CardType type,
        string promptText,
        string canonicalAnswer,
        DateOnly createdOn,
        DateTimeOffset createdAt,
        long? topicId = null,
        long? noteId = null,
        string? contextNotes = null)
    {
        UserId = userId;
        Type = type;
        PromptText = promptText;
        CanonicalAnswer = canonicalAnswer;
        TopicId = topicId;
        NoteId = noteId;
        ContextNotes = contextNotes;
        CreatedAt = createdAt;
        State = SpacedRepetitionState.New(createdOn);
    }

    /// <summary>Applies one SM-2 review and advances the card's scheduling state.</summary>
    public void Review(int grade, DateOnly today) => State = Sm2Engine.Review(State, grade, today);

    public void AttachAudio(string audioUrl) => AudioUrl = audioUrl;
}
