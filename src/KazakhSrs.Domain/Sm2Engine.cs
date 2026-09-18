namespace KazakhSrs.Domain;

/// <summary>
/// SM-2 (SuperMemo 2) spaced-repetition scheduler.
/// Pure, stateless, no I/O — takes the current state and a grade, returns the next state.
/// Callers (repositories, use cases) own persistence; this class only does the math.
/// </summary>
public static class Sm2Engine
{
    /// <summary>Lowest ease factor SM-2 allows — prevents intervals collapsing to nothing on repeated failures.</summary>
    public const double MinEaseFactor = 1.3;

    /// <summary>Grades below this count as "forgot" and reset the repetition streak.</summary>
    public const int PassingGrade = 3;

    /// <summary>
    /// Applies one review to the given state and returns the resulting state.
    /// </summary>
    /// <param name="current">The card's state going into this review.</param>
    /// <param name="grade">Recall quality, 0 (total blank) to 5 (perfect, instant recall).</param>
    /// <param name="today">The date the review happened (injected for testability).</param>
    public static SpacedRepetitionState Review(SpacedRepetitionState current, int grade, DateOnly today)
    {
        if (grade is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(grade), grade, "SM-2 grade must be between 0 and 5.");

        var newEase = Math.Max(MinEaseFactor, current.EaseFactor + (0.1 - (5 - grade) * (0.08 + (5 - grade) * 0.02)));

        if (grade < PassingGrade)
        {
            // Forgot it: restart the repetition streak, but keep the (slightly lowered) ease —
            // a card that's been forgotten once stays "harder" going forward, it doesn't fully reset.
            return current with
            {
                Repetitions = 0,
                IntervalDays = 1,
                EaseFactor = newEase,
                DueDate = today.AddDays(1),
            };
        }

        var newRepetitions = current.Repetitions + 1;
        var newInterval = current.Repetitions switch
        {
            0 => 1,
            1 => 6,
            _ => (int)Math.Round(current.IntervalDays * current.EaseFactor, MidpointRounding.AwayFromZero),
        };

        return current with
        {
            Repetitions = newRepetitions,
            IntervalDays = newInterval,
            EaseFactor = newEase,
            DueDate = today.AddDays(newInterval),
        };
    }
}