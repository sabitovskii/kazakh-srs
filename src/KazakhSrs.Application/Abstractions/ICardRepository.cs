using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.Abstractions;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Card>> GetDueCardsAsync(long userId, DateOnly today, CancellationToken ct = default);

    /// <summary>Adds a new card and persists it immediately.</summary>
    Task AddAsync(Card card, CancellationToken ct = default);
}
