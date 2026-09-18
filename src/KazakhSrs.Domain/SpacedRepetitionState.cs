namespace KazakhSrs.Domain;

/// <summary>
/// SM-2 scheduling state for a single card. Immutable — every review produces a new state.
/// </summary>
public sealed record SpacedRepetitionState(
    int Repetitions,
    int IntervalDays,
    double EaseFactor,
    DateOnly DueDate)
{
    /// <summary>The state a brand-new card starts in (never reviewed).</summary>
    public static SpacedRepetitionState New(DateOnly today) =>
        new(Repetitions: 0, IntervalDays: 0, EaseFactor: 2.5, DueDate: today);
}
