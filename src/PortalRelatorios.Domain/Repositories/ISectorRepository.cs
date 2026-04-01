using PortalRelatorios.Domain.Entities;

namespace PortalRelatorios.Domain.Repositories;

public interface ISectorRepository
{
    Task<IReadOnlyList<Sector>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
