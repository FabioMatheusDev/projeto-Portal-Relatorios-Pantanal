using Microsoft.EntityFrameworkCore;
using PortalRelatorios.Domain.Entities;

namespace PortalRelatorios.Infrastructure.Persistence;

public static class DatabaseSeed
{
    private static readonly string[] DefaultSectors =
    {
        "Administrativo",
        "Comercial",
        "Financeiro",
        "Suprimentos",
        "Controladoria",
        "Crédito",
        "Gerentes",
        "Lojas",
        "TI",
        "Auditoria"
    };

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (!await db.Sectors.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (var name in DefaultSectors)
            {
                await db.Sectors.AddAsync(new Sector { Id = Guid.NewGuid(), Name = name }, cancellationToken)
                    .ConfigureAwait(false);
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        if (!await db.Users.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            await db.Users.AddAsync(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "demo.admin",
                    Name = "Administrador Demo",
                    Email = "admin@mock.local",
                    IsAdmin = true
                },
                cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
