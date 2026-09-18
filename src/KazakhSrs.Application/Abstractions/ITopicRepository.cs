using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.Abstractions;

public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Topic>> GetByUserIdAsync(long userId, CancellationToken ct = default);

    /// <summary>Adds a new topic and persists it immediately.</summary>
    Task AddAsync(Topic topic, CancellationToken ct = default);
}
