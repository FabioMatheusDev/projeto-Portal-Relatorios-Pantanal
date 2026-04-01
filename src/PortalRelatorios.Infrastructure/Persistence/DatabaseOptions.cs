namespace PortalRelatorios.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public bool UseInMemoryDatabase { get; set; }
    public string? HanaConnectionString { get; set; }
}
