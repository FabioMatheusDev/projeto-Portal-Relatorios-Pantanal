namespace PortalRelatorios.Infrastructure.Sap;

/// <summary>Mapeamento chave lógica → caminho relativo ao BaseUrl do Service Layer (ex.: b1s/v1/...).</summary>
public sealed class ReportEndpointOptions
{
    public const string SectionName = "ReportEndpoints";

    public Dictionary<string, string> Routes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
