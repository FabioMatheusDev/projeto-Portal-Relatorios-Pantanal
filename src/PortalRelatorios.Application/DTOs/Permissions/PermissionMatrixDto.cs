namespace PortalRelatorios.Application.DTOs.Permissions;

public sealed class PermissionMatrixDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public IReadOnlyList<SectorPermissionRowDto> Sectors { get; set; } = Array.Empty<SectorPermissionRowDto>();
}

public sealed class SectorPermissionRowDto
{
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public bool CanView { get; set; }
}

public sealed class UpdatePermissionsRequestDto
{
    public Guid UserId { get; set; }
    public IReadOnlyList<SectorPermissionUpdateDto> Permissions { get; set; } = Array.Empty<SectorPermissionUpdateDto>();
}

public sealed class SectorPermissionUpdateDto
{
    public Guid SectorId { get; set; }
    public bool CanView { get; set; }
}
