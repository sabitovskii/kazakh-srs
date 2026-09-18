using KazakhSrs.Domain.Entities;
using KazakhSrs.Domain.Enums;

namespace KazakhSrs.Domain.Tests;

public class CardTests
{
    private static readonly DateOnly Day0 = new(2026, 1, 1);
    private static readonly DateTimeOffset CreatedAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void NewCard_StartsWithFreshSm2State()
    {
        var card = new Card(userId: 1, type: CardType.Qa, promptText: "Привет", canonicalAnswer: "Сәлем", createdOn: Day0, createdAt: CreatedAt);

        Assert.Equal(0, card.State.Repetitions);
        Assert.Equal(Day0, card.State.DueDate);
    }

    [Fact]
    public void Review_DelegatesToSm2Engine()
    {
        var card = new Card(userId: 1, type: CardType.Qa, promptText: "Привет", canonicalAnswer: "Сәлем", createdOn: Day0, createdAt: CreatedAt);

        card.Review(grade: 4, today: Day0);

        Assert.Equal(1, card.State.Repetitions);
        Assert.Equal(Day0.AddDays(1), card.State.DueDate);
    }
}
