namespace PortalRelatorios.Application.Abstractions;

public sealed record AdUserInfo(string Username, string DisplayName, string? Email);

public interface IActiveDirectoryAuthService
{
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<AdUserInfo?> GetUserFromAdAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdUserInfo>> GetAllActiveUsersAsync(CancellationToken cancellationToken = default);
}
