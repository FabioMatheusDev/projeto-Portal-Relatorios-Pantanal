using PortalRelatorios.Application.DTOs.Permissions;
using PortalRelatorios.Application.DTOs.Sectors;
using PortalRelatorios.Application.Abstractions;
using PortalRelatorios.Domain.Entities;
using PortalRelatorios.Domain.Repositories;

namespace PortalRelatorios.Application.Services;

public sealed class PermissionAppService : IPermissionAppService
{
    private readonly ISectorRepository _sectors;
    private readonly IUserRepository _users;
    private readonly IPermissionRepository _permissions;
    private readonly IActiveDirectoryAuthService _ad;
    private readonly IUnitOfWork _uow;

    public PermissionAppService(
        ISectorRepository sectors,
        IUserRepository users,
        IPermissionRepository permissions,
        IActiveDirectoryAuthService ad,
        IUnitOfWork uow)
    {
        _sectors = sectors;
        _users = users;
        _permissions = permissions;
        _ad = ad;
        _uow = uow;
    }

    public async Task<IReadOnlyList<SectorDto>> GetSectorsAsync(CancellationToken cancellationToken = default)
    {
        var list = await _sectors.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return list.Select(s => new SectorDto { Id = s.Id, Name = s.Name }).ToList();
    }

    public async Task<PermissionMatrixDto?> GetMatrixAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null)
            return null;

        var sectors = await _sectors.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var existing = await _permissions.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        var bySector = existing.ToDictionary(p => p.SectorId);

        var rows = sectors.Select(s => new SectorPermissionRowDto
        {
            SectorId = s.Id,
            SectorName = s.Name,
            CanView = bySector.TryGetValue(s.Id, out var p) && p.CanView
        }).ToList();

        return new PermissionMatrixDto
        {
            UserId = user.Id,
            Username = user.Username,
            Sectors = rows
        };
    }

    public async Task UpdateMatrixAsync(UpdatePermissionsRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (user is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        foreach (var row in request.Permissions)
        {
            await _permissions.UpsertAsync(request.UserId, row.SectorId, row.CanView, cancellationToken)
                .ConfigureAwait(false);
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AdUserListItemDto>> GetActiveDirectoryUsersAsync(CancellationToken cancellationToken = default)
    {
        var adUsers = await _ad.GetAllActiveUsersAsync(cancellationToken).ConfigureAwait(false);
        var result = new List<AdUserListItemDto>();

        foreach (var u in adUsers.OrderBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase))
        {
            var portal = await _users.GetByUsernameAsync(u.Username, cancellationToken).ConfigureAwait(false);
            result.Add(new AdUserListItemDto
            {
                Username = u.Username,
                Name = u.DisplayName,
                Email = u.Email,
                PortalUserId = portal?.Id
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<Guid>> GetViewableSectorIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var perms = await _permissions.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return perms.Where(p => p.CanView).Select(p => p.SectorId).ToList();
    }

    public async Task<Guid?> EnsurePortalUserFromAdAsync(string username, CancellationToken cancellationToken = default)
    {
        var adUser = await _ad.GetUserFromAdAsync(username, cancellationToken).ConfigureAwait(false);
        if (adUser is null)
            return null;

        var existing = await _users.GetByUsernameAsync(adUser.Username, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
            return existing.Id;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = adUser.Username,
            Name = adUser.DisplayName,
            Email = adUser.Email,
            IsAdmin = false
        };
        await _users.AddAsync(user, cancellationToken).ConfigureAwait(false);
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return user.Id;
    }
}
