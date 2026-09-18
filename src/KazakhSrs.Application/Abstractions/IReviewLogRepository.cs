using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.Abstractions;

public interface IReviewLogRepository
{
    /// <summary>
    /// Adds a review-log entry and persists it immediately — together with any other
    /// pending change on the same scoped DbContext (e.g. a Card mutated earlier in the
    /// same use case), since SaveChanges commits everything the context is tracking.
    /// </summary>
    Task AddAsync(ReviewLog reviewLog, CancellationToken ct = default);
}
