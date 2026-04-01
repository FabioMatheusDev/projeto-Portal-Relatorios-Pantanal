using PortalRelatorios.Application.DTOs.Auth;

namespace PortalRelatorios.Application.Services;

public interface IAuthAppService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<UserSummaryDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
