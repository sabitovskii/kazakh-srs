namespace KazakhSrs.Domain.Entities;

public class ReviewLog
{
    public long Id { get; private set; }
    public long CardId { get; private set; }
    public long UserId { get; private set; }
    public DateTimeOffset ReviewedAt { get; private set; }
    public int Grade { get; private set; }
    public string? UserResponseText { get; private set; }
    public string? RawAudioRef { get; private set; }

    private ReviewLog()
    {
    } // EF Core

    public ReviewLog(long cardId, long userId, int grade, DateTimeOffset reviewedAt, string? userResponseText = null,
        string? rawAudioRef = null)
    {
        if (grade is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(grade), grade, "SM-2 grade must be between 0 and 5.");

        CardId = cardId;
        UserId = userId;
        Grade = grade;
        ReviewedAt = reviewedAt;
        UserResponseText = userResponseText;
        RawAudioRef = rawAudioRef;
    }
}