using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KazakhSrs.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly KazakhSrsDbContext _db;

    public CardRepository(KazakhSrsDbContext db) => _db = db;

    public Task<Card?> GetByIdAsync(long id, CancellationToken ct = default) =>
        _db.Cards.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Card>> GetDueCardsAsync(long userId, DateOnly today, CancellationToken ct = default) =>
        await _db.Cards
            .Where(c => c.UserId == userId && c.State.DueDate <= today)
            .ToListAsync(ct);

    public async Task AddAsync(Card card, CancellationToken ct = default)
    {
        await _db.Cards.AddAsync(card, ct);
        await _db.SaveChangesAsync(ct);
    }
}
