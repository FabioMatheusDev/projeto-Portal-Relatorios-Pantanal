using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRelatorios.Application.DTOs.Users;
using PortalRelatorios.Application.Services;

namespace PortalRelatorios.API.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IPermissionAppService _permissions;

    public UsersController(IPermissionAppService permissions)
    {
        _permissions = permissions;
    }

    [HttpGet("active-directory")]
    public async Task<IActionResult> ActiveDirectory(CancellationToken cancellationToken)
    {
        var list = await _permissions.GetActiveDirectoryUsersAsync(cancellationToken).ConfigureAwait(false);
        return Ok(list);
    }

    [HttpPost("ensure")]
    public async Task<IActionResult> Ensure([FromBody] EnsureUserRequestDto request, CancellationToken cancellationToken)
    {
        var id = await _permissions.EnsurePortalUserFromAdAsync(request.Username, cancellationToken).ConfigureAwait(false);
        if (id is null)
            return NotFound(new { message = "Usuário não encontrado no Active Directory." });
        return Ok(new { userId = id.Value });
    }
}
