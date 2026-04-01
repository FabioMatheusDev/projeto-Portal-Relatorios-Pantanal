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

        await EnsureAdminUserAsync(db, cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureAdminUserAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        var admin = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin", cancellationToken)
            .ConfigureAwait(false);
        if (admin is null)
        {
            await db.Users.AddAsync(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Name = "Administrador",
                    Email = "admin@local",
                    IsAdmin = true
                },
                cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return;
        }

        if (admin.IsAdmin)
            return;

        admin.IsAdmin = true;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
