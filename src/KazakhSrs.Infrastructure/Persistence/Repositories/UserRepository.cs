using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KazakhSrs.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly KazakhSrsDbContext _db;

    public UserRepository(KazakhSrsDbContext db) => _db = db;

    public Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }
}
