using System.Security.Claims;

namespace PortalRelatorios.Application.Abstractions;

public interface IJwtTokenService
{
    string CreateToken(Guid userId, string username, string name, string? email, bool isAdmin);
    ClaimsPrincipal? ValidateToken(string token);
}
