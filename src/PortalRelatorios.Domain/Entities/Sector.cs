namespace PortalRelatorios.Domain.Entities;

public class Sector
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
