using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default);

    /// <summary>Adds a new user and persists it immediately.</summary>
    Task AddAsync(User user, CancellationToken ct = default);
}
