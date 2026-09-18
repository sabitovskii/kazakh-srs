namespace KazakhSrs.Domain.Entities;

public class User
{
    public long Id { get; private set; }
    public long TelegramId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { } // EF Core

    public User(long telegramId, DateTimeOffset createdAt)
    {
        TelegramId = telegramId;
        CreatedAt = createdAt;
    }
}
