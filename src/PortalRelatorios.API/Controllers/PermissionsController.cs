using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRelatorios.Application.DTOs.Permissions;
using PortalRelatorios.Application.Services;

namespace PortalRelatorios.API.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
public sealed class PermissionsController : ControllerBase
{
    private readonly IPermissionAppService _permissions;

    public PermissionsController(IPermissionAppService permissions)
    {
        _permissions = permissions;
    }

    [HttpGet("matrix/{userId:guid}")]
    public async Task<IActionResult> GetMatrix(Guid userId, CancellationToken cancellationToken)
    {
        var matrix = await _permissions.GetMatrixAsync(userId, cancellationToken).ConfigureAwait(false);
        return matrix is null ? NotFound() : Ok(matrix);
    }

    [HttpPut("matrix")]
    public async Task<IActionResult> UpdateMatrix([FromBody] UpdatePermissionsRequestDto request, CancellationToken cancellationToken)
    {
        await _permissions.UpdateMatrixAsync(request, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}
