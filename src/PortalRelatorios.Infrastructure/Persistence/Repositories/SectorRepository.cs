using Microsoft.EntityFrameworkCore;
using PortalRelatorios.Domain.Entities;
using PortalRelatorios.Domain.Repositories;

namespace PortalRelatorios.Infrastructure.Persistence.Repositories;

public sealed class SectorRepository : ISectorRepository
{
    private readonly AppDbContext _db;

    public SectorRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Sectors.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Sector>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _db.Sectors.AsNoTracking().OrderBy(s => s.Name).ToListAsync(cancellationToken).ConfigureAwait(false);
        return list;
    }
}
