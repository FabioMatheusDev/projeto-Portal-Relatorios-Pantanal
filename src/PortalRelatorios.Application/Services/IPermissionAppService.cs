using PortalRelatorios.Application.DTOs.Permissions;
using PortalRelatorios.Application.DTOs.Sectors;

namespace PortalRelatorios.Application.Services;

public interface IPermissionAppService
{
    Task<IReadOnlyList<SectorDto>> GetSectorsAsync(CancellationToken cancellationToken = default);
    Task<PermissionMatrixDto?> GetMatrixAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateMatrixAsync(UpdatePermissionsRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdUserListItemDto>> GetActiveDirectoryUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetViewableSectorIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Guid?> EnsurePortalUserFromAdAsync(string username, CancellationToken cancellationToken = default);
}

public sealed class AdUserListItemDto
{
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public Guid? PortalUserId { get; set; }
}
