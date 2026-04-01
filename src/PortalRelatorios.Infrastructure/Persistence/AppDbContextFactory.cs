using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sap.EntityFrameworkCore.Hana;

namespace PortalRelatorios.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs =
            Environment.GetEnvironmentVariable("PORTAL_HANA_DESIGN_CS")
            ?? "Server=localhost:30015;UserID=SYSTEM;Password=design-time;Current Schema=PORTAL";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseHana(cs)
            .Options;
        return new AppDbContext(options);
    }
}
