namespace PortalRelatorios.Infrastructure.Sap;

public sealed class SapServiceLayerOptions
{
    public const string SectionName = "Sap";

    public string BaseUrl { get; set; } = "https://localhost:50000/";
    public string CompanyDB { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseMockResponses { get; set; }
}
