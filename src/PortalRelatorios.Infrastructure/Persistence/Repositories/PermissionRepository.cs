using Microsoft.EntityFrameworkCore;
using PortalRelatorios.Domain.Entities;
using PortalRelatorios.Domain.Repositories;

namespace PortalRelatorios.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _db;

    public PermissionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Permissions.AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpsertAsync(Guid userId, Guid sectorId, bool canView, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Permissions.FirstOrDefaultAsync(
            p => p.UserId == userId && p.SectorId == sectorId, cancellationToken).ConfigureAwait(false);

        if (existing is null)
        {
            await _db.Permissions.AddAsync(new Permission
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SectorId = sectorId,
                CanView = canView
            }, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            existing.CanView = canView;
        }
    }
}
