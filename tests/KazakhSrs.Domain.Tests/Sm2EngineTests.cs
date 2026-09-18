using KazakhSrs.Domain;

namespace KazakhSrs.Domain.Tests;

public class Sm2EngineTests
{
    private static readonly DateOnly Day0 = new(2026, 1, 1);

    [Fact]
    public void FirstSuccessfulReview_SetsIntervalToOneDay()
    {
        var state = SpacedRepetitionState.New(Day0);

        var result = Sm2Engine.Review(state, grade: 4, today: Day0);

        Assert.Equal(1, result.Repetitions);
        Assert.Equal(1, result.IntervalDays);
        Assert.Equal(Day0.AddDays(1), result.DueDate);
    }

    [Fact]
    public void SecondSuccessfulReview_SetsIntervalToSixDays()
    {
        var state = SpacedRepetitionState.New(Day0);
        var afterFirst = Sm2Engine.Review(state, grade: 4, today: Day0);

        var afterSecond = Sm2Engine.Review(afterFirst, grade: 4, today: Day0.AddDays(1));

        Assert.Equal(2, afterSecond.Repetitions);
        Assert.Equal(6, afterSecond.IntervalDays);
    }

    [Fact]
    public void ThirdSuccessfulReview_MultipliesPreviousIntervalByEaseFactor()
    {
        var state = SpacedRepetitionState.New(Day0);
        var s1 = Sm2Engine.Review(state, grade: 4, today: Day0);
        var s2 = Sm2Engine.Review(s1, grade: 4, today: Day0.AddDays(1));

        var s3 = Sm2Engine.Review(s2, grade: 4, today: Day0.AddDays(7));

        // interval = round(6 * ease-after-second-review), not the pre-review ease
        var expectedInterval = (int)Math.Round(s2.IntervalDays * s2.EaseFactor, MidpointRounding.AwayFromZero);
        Assert.Equal(expectedInterval, s3.IntervalDays);
        Assert.Equal(3, s3.Repetitions);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void FailingGrade_ResetsRepetitionsAndIntervalToOneDay(int failingGrade)
    {
        var state = SpacedRepetitionState.New(Day0);
        var afterFirst = Sm2Engine.Review(state, grade: 4, today: Day0);
        var afterSecond = Sm2Engine.Review(afterFirst, grade: 4, today: Day0.AddDays(1));

        var afterForgetting = Sm2Engine.Review(afterSecond, grade: failingGrade, today: Day0.AddDays(7));

        Assert.Equal(0, afterForgetting.Repetitions);
        Assert.Equal(1, afterForgetting.IntervalDays);
        Assert.Equal(Day0.AddDays(8), afterForgetting.DueDate);
    }

    [Fact]
    public void FailingGrade_DoesNotFullyResetEaseFactor()
    {
        // A forgotten card should come out "harder" than a fresh one (ease < 2.5),
        // not reset back to the default — that's what makes it grow more slowly next time.
        var state = SpacedRepetitionState.New(Day0);

        var afterForgetting = Sm2Engine.Review(state, grade: 0, today: Day0);

        Assert.True(afterForgetting.EaseFactor < 2.5);
    }

    [Fact]
    public void EaseFactor_NeverDropsBelowTheSm2Floor()
    {
        var state = SpacedRepetitionState.New(Day0);
        var today = Day0;

        // Hammer it with the worst possible grade repeatedly.
        for (var i = 0; i < 50; i++)
        {
            state = Sm2Engine.Review(state, grade: 0, today: today);
            today = today.AddDays(1);
        }

        Assert.Equal(Sm2Engine.MinEaseFactor, state.EaseFactor);
    }

    [Fact]
    public void EasyGrade_GrowsEaseFactorAboveDefault()
    {
        var state = SpacedRepetitionState.New(Day0);

        var result = Sm2Engine.Review(state, grade: 5, today: Day0);

        Assert.True(result.EaseFactor > 2.5);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void OutOfRangeGrade_Throws(int invalidGrade)
    {
        var state = SpacedRepetitionState.New(Day0);

        Assert.Throws<ArgumentOutOfRangeException>(() => Sm2Engine.Review(state, invalidGrade, Day0));
    }
}
