namespace PortalRelatorios.Application.Abstractions;

public interface ISapServiceLayerService
{
    Task EnsureSessionAsync(CancellationToken cancellationToken = default);
    Task<string?> GetReportDataAsync(string endpointKey, IReadOnlyDictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default);
}
