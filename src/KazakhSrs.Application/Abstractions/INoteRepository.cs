using KazakhSrs.Domain.Entities;

namespace KazakhSrs.Application.Abstractions;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(long id, CancellationToken ct = default);

    /// <summary>Adds a new note and persists it immediately.</summary>
    Task AddAsync(Note note, CancellationToken ct = default);
}
