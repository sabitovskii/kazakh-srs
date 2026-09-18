namespace KazakhSrs.Domain.Entities;

public class Topic
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private Topic() { } // EF Core

    public Topic(long userId, string name, DateTimeOffset createdAt)
    {
        UserId = userId;
        Name = name;
        CreatedAt = createdAt;
    }
}
