using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PortalRelatorios.Application.Abstractions;

namespace PortalRelatorios.Infrastructure.Identity;

public sealed class ActiveDirectoryAuthService : IActiveDirectoryAuthService
{
    private readonly LdapOptions _options;
    private readonly ILogger<ActiveDirectoryAuthService> _logger;

    public ActiveDirectoryAuthService(IOptions<LdapOptions> options, ILogger<ActiveDirectoryAuthService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (_options.UseMock)
        {
            _logger.LogInformation("LDAP mock: validação aceita para {User}", username);
            return Task.FromResult(true);
        }

        return Task.Run(() => ValidateCredentialsInternal(username, password), cancellationToken);
    }

    private bool ValidateCredentialsInternal(string username, string password)
    {
        try
        {
            var dn = FindUserDn(username);
            if (string.IsNullOrEmpty(dn))
                return false;

            using var connection = CreateConnection();
            connection.Credential = new NetworkCredential(dn, password);
            connection.AuthType = AuthType.Basic;
            connection.Bind();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao validar credenciais LDAP para {User}", username);
            return false;
        }
    }

    public Task<AdUserInfo?> GetUserFromAdAsync(string username, CancellationToken cancellationToken = default)
    {
        if (_options.UseMock)
        {
            var u = username.Trim();
            return Task.FromResult<AdUserInfo?>(new AdUserInfo(u, $"Usuário {u}", $"{u}@mock.local"));
        }

        return Task.Run(() => GetUserFromAdInternal(username), cancellationToken);
    }

    private AdUserInfo? GetUserFromAdInternal(string username)
    {
        try
        {
            using var connection = CreateConnection();
            BindServiceAccount(connection);

            var filter = string.Format(_options.UserSearchFilter, EscapeLdapFilter(username));
            var request = new SearchRequest(_options.BaseDn, filter, SearchScope.Subtree, "sAMAccountName", "displayName", "mail");
            var response = (SearchResponse)connection.SendRequest(request);
            if (response.Entries.Count == 0)
                return null;

            var entry = response.Entries[0];
            var sam = entry.Attributes["sAMAccountName"]?[0]?.ToString() ?? username;
            var display = entry.Attributes["displayName"]?[0]?.ToString() ?? sam;
            var mail = entry.Attributes["mail"]?[0]?.ToString();
            return new AdUserInfo(sam, display, mail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário AD {User}", username);
            return null;
        }
    }

    public Task<IReadOnlyList<AdUserInfo>> GetAllActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        if (_options.UseMock)
        {
            IReadOnlyList<AdUserInfo> mock =
            [
                new("demo.admin", "Administrador Demo", "admin@mock.local"),
                new("demo.user", "Usuário Demo", "user@mock.local")
            ];
            return Task.FromResult(mock);
        }

        return Task.Run(GetAllActiveUsersInternal, cancellationToken);
    }

    private IReadOnlyList<AdUserInfo> GetAllActiveUsersInternal()
    {
        var list = new List<AdUserInfo>();
        try
        {
            using var connection = CreateConnection();
            BindServiceAccount(connection);

            var request = new SearchRequest(
                _options.BaseDn,
                _options.ActiveUsersFilter,
                SearchScope.Subtree,
                "sAMAccountName", "displayName", "mail");

            var response = (SearchResponse)connection.SendRequest(request);
            foreach (SearchResultEntry entry in response.Entries)
            {
                var sam = entry.Attributes["sAMAccountName"]?[0]?.ToString();
                if (string.IsNullOrEmpty(sam))
                    continue;
                var display = entry.Attributes["displayName"]?[0]?.ToString() ?? sam;
                var mail = entry.Attributes["mail"]?[0]?.ToString();
                list.Add(new AdUserInfo(sam, display, mail));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar usuários AD");
        }

        return list;
    }

    private string? FindUserDn(string username)
    {
        using var connection = CreateConnection();
        BindServiceAccount(connection);

        var filter = string.Format(_options.UserSearchFilter, EscapeLdapFilter(username));
        var request = new SearchRequest(_options.BaseDn, filter, SearchScope.Subtree);
        var response = (SearchResponse)connection.SendRequest(request);
        return response.Entries.Count == 0 ? null : response.Entries[0].DistinguishedName;
    }

    private void BindServiceAccount(LdapConnection connection)
    {
        if (string.IsNullOrEmpty(_options.BindUserDn))
            throw new InvalidOperationException("Ldap:BindUserDn é obrigatório para buscas LDAP.");

        connection.Credential = new NetworkCredential(_options.BindUserDn, _options.BindPassword);
        connection.AuthType = AuthType.Basic;
        connection.Bind();
    }

    private LdapConnection CreateConnection()
    {
        var host = _options.Server;
        var port = _options.Port;
        if (Uri.TryCreate(_options.Server, UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            if (uri.Port > 0)
                port = uri.Port;
        }

        var identifier = new LdapDirectoryIdentifier(host, port, false, false);
        var connection = new LdapConnection(identifier)
        {
            SessionOptions =
            {
                ProtocolVersion = 3,
                SecureSocketLayer = _options.UseSsl
            }
        };
        return connection;
    }

    private static string EscapeLdapFilter(string input)
    {
        return input.Replace("\\", "\\5c", StringComparison.Ordinal)
            .Replace("*", "\\2a", StringComparison.Ordinal)
            .Replace("(", "\\28", StringComparison.Ordinal)
            .Replace(")", "\\29", StringComparison.Ordinal);
    }
}
