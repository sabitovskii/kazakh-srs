using KazakhSrs.Application.Abstractions;
using KazakhSrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KazakhSrs.Infrastructure.Persistence.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly KazakhSrsDbContext _db;

    public NoteRepository(KazakhSrsDbContext db) => _db = db;

    public Task<Note?> GetByIdAsync(long id, CancellationToken ct = default) =>
        _db.Notes.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task AddAsync(Note note, CancellationToken ct = default)
    {
        await _db.Notes.AddAsync(note, ct);
        await _db.SaveChangesAsync(ct);
    }
}
