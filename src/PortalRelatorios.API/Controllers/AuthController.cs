using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRelatorios.Application.DTOs.Auth;
using PortalRelatorios.Application.Services;
using PortalRelatorios.CrossCutting.Security;

namespace PortalRelatorios.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthAppService _auth;

    public AuthController(IAuthAppService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _auth.LoginAsync(request, cancellationToken).ConfigureAwait(false);
        if (result is null)
            return Unauthorized(new { message = "Credenciais inválidas." });
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserSummaryDto>> Me(CancellationToken cancellationToken)
    {
        var id = GetUserId();
        if (id is null)
            return Unauthorized();

        var user = await _auth.GetProfileAsync(id.Value, cancellationToken).ConfigureAwait(false);
        return user is null ? NotFound() : Ok(user);
    }

    private Guid? GetUserId()
    {
        var v = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(PortalClaims.UserId);
        return Guid.TryParse(v, out var id) ? id : null;
    }
}
