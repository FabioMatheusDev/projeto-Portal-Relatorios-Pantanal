using System.Net;

namespace PortalRelatorios.Infrastructure.Sap;

public sealed class SapCookieContainer
{
    public CookieContainer Cookies { get; } = new();
}
