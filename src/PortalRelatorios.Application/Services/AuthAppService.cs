using Microsoft.Extensions.Options;
using PortalRelatorios.Application.Abstractions;
using PortalRelatorios.Application.Configuration;
using PortalRelatorios.Application.DTOs.Auth;
using PortalRelatorios.Domain.Entities;
using PortalRelatorios.Domain.Repositories;

namespace PortalRelatorios.Application.Services;

public sealed class AuthAppService : IAuthAppService
{
    private readonly IActiveDirectoryAuthService _ad;
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IUnitOfWork _uow;
    private readonly JwtOptions _jwtOptions;

    public AuthAppService(
        IActiveDirectoryAuthService ad,
        IUserRepository users,
        IJwtTokenService jwt,
        IUnitOfWork uow,
        IOptions<JwtOptions> jwtOptions)
    {
        _ad = ad;
        _users = users;
        _jwt = jwt;
        _uow = uow;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(request.Password))
            return null;

        var valid = await _ad.ValidateCredentialsAsync(username, request.Password, cancellationToken).ConfigureAwait(false);
        if (!valid)
            return null;

        var adUser = await _ad.GetUserFromAdAsync(username, cancellationToken).ConfigureAwait(false);
        if (adUser is null)
            return null;

        var user = await _users.GetByUsernameAsync(adUser.Username, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = adUser.Username,
                Name = adUser.DisplayName,
                Email = adUser.Email,
                IsAdmin = false
            };
            await _users.AddAsync(user, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            user.Name = adUser.DisplayName;
            user.Email = adUser.Email;
            await _users.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var token = _jwt.CreateToken(user.Id, user.Username, user.Name, user.Email, user.IsAdmin);
        var expiresIn = _jwtOptions.ExpireMinutes * 60;

        return new LoginResponseDto
        {
            AccessToken = token,
            ExpiresIn = expiresIn,
            User = new UserSummaryDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            }
        };
    }

    public async Task<UserSummaryDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null)
            return null;

        return new UserSummaryDto
        {
            Id = user.Id,
            Username = user.Username,
            Name = user.Name,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };
    }
}
