using PortalRelatorios.Domain.Entities;

namespace PortalRelatorios.Domain.Repositories;

public interface IPermissionRepository
{
    Task<IReadOnlyList<Permission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpsertAsync(Guid userId, Guid sectorId, bool canView, CancellationToken cancellationToken = default);
}
