namespace PortalRelatorios.Infrastructure.Identity;

public sealed class LdapOptions
{
    public const string SectionName = "Ldap";

    public bool UseMock { get; set; }
    public string Server { get; set; } = "ldap://localhost";
    public int Port { get; set; } = 389;
    public bool UseSsl { get; set; }
    public string BaseDn { get; set; } = string.Empty;
    public string? BindUserDn { get; set; }
    public string? BindPassword { get; set; }
    /// <summary>Filtro LDAP com placeholder {0} para o nome de usuário (sAMAccountName).</summary>
    public string UserSearchFilter { get; set; } = "(&(objectClass=user)(objectCategory=person)(sAMAccountName={0}))";
    public string ActiveUsersFilter { get; set; } =
        "(&(objectClass=user)(objectCategory=person)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
    public int SearchPageSize { get; set; } = 500;
}
