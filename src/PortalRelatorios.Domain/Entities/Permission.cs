namespace PortalRelatorios.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SectorId { get; set; }
    public bool CanView { get; set; }

    public User User { get; set; } = null!;
    public Sector Sector { get; set; } = null!;
}
