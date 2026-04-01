using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRelatorios.Application.Services;
using PortalRelatorios.CrossCutting.Security;

namespace PortalRelatorios.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class SectorsController : ControllerBase
{
    private readonly IPermissionAppService _permissions;

    public SectorsController(IPermissionAppService permissions)
    {
        _permissions = permissions;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await _permissions.GetSectorsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(list);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var id = GetUserId();
        if (id is null)
            return Unauthorized();

        var ids = await _permissions.GetViewableSectorIdsAsync(id.Value, cancellationToken).ConfigureAwait(false);
        return Ok(ids);
    }

    private Guid? GetUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(PortalClaims.UserId);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
