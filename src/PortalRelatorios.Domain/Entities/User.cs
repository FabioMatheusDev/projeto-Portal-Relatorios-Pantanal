namespace PortalRelatorios.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
