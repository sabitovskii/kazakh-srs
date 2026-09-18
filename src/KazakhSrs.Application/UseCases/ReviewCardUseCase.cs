using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.UseCases;

/// <summary>
/// Applies one SM-2 review to a card: loads it, advances its scheduling state via the
/// domain's own Sm2Engine, and records the attempt in the review log.
///
/// No explicit Unit of Work here: card and reviewLogs share the same scoped DbContext
/// (see KazakhSrsDbContext / DI registration), so the single SaveChanges inside
/// IReviewLogRepository.AddAsync commits both the mutated Card and the new ReviewLog
/// in one transaction.
/// </summary>
public sealed class ReviewCardUseCase
{
    private readonly ICardRepository _cards;
    private readonly IReviewLogRepository _reviewLogs;

    public ReviewCardUseCase(ICardRepository cards, IReviewLogRepository reviewLogs)
    {
        _cards = cards;
        _reviewLogs = reviewLogs;
    }

    public async Task ExecuteAsync(long cardId, int grade, DateOnly today, string? userResponseText = null, CancellationToken ct = default)
    {
        var card = await _cards.GetByIdAsync(cardId, ct)
            ?? throw new InvalidOperationException($"Card {cardId} not found.");

        card.Review(grade, today);

        var log = new ReviewLog(card.Id, card.UserId, grade, DateTimeOffset.UtcNow, userResponseText);
        await _reviewLogs.AddAsync(log, ct);
    }
}
