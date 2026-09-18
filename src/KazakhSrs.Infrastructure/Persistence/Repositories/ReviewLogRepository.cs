using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Infrastructure.Persistence.Repositories;

public class ReviewLogRepository : IReviewLogRepository
{
    private readonly KazakhSrsDbContext _db;

    public ReviewLogRepository(KazakhSrsDbContext db) => _db = db;

    public async Task AddAsync(ReviewLog reviewLog, CancellationToken ct = default)
    {
        // SaveChanges here also flushes any other pending change on this scoped DbContext —
        // e.g. a Card mutated earlier in the same use case (ReviewCardUseCase relies on this).
        await _db.ReviewLogs.AddAsync(reviewLog, ct);
        await _db.SaveChangesAsync(ct);
    }
}
