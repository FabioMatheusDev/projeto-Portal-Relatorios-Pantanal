namespace PortalRelatorios.Infrastructure.Sap;

public sealed class SapHttpHandler : HttpClientHandler
{
    public SapHttpHandler(SapCookieContainer store)
    {
        UseCookies = true;
        CookieContainer = store.Cookies;
    }
}
