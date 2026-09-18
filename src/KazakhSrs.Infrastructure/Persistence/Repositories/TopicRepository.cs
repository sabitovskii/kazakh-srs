using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KazakhSrs.Infrastructure.Persistence.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly KazakhSrsDbContext _db;

    public TopicRepository(KazakhSrsDbContext db) => _db = db;

    public Task<Topic?> GetByIdAsync(long id, CancellationToken ct = default) =>
        _db.Topics.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Topic>> GetByUserIdAsync(long userId, CancellationToken ct = default) =>
        await _db.Topics.Where(t => t.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(Topic topic, CancellationToken ct = default)
    {
        await _db.Topics.AddAsync(topic, ct);
        await _db.SaveChangesAsync(ct);
    }
}
