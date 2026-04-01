using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PortalRelatorios.Application.Abstractions;

namespace PortalRelatorios.Infrastructure.Sap;

public sealed class SapServiceLayerService : ISapServiceLayerService
{
    private readonly HttpClient _http;
    private readonly SapServiceLayerOptions _options;
    private readonly ReportEndpointOptions _routes;
    private readonly ILogger<SapServiceLayerService> _logger;
    private readonly SemaphoreSlim _sessionLock = new(1, 1);
    private bool _sessionReady;

    public SapServiceLayerService(
        HttpClient http,
        IOptions<SapServiceLayerOptions> options,
        IOptions<ReportEndpointOptions> routes,
        ILogger<SapServiceLayerService> logger)
    {
        _http = http;
        _options = options.Value;
        _routes = routes.Value;
        _logger = logger;
    }

    public async Task EnsureSessionAsync(CancellationToken cancellationToken = default)
    {
        if (_options.UseMockResponses)
        {
            _sessionReady = true;
            return;
        }

        await _sessionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_sessionReady)
                return;

            var payload = new
            {
                CompanyDB = _options.CompanyDB,
                UserName = _options.UserName,
                Password = _options.Password
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("b1s/v1/Login", content, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Falha no login SAP Service Layer: {Status} {Body}", response.StatusCode, body);
                throw new InvalidOperationException("Não foi possível autenticar no SAP Service Layer.");
            }

            _sessionReady = true;
        }
        finally
        {
            _sessionLock.Release();
        }
    }

    public async Task<string?> GetReportDataAsync(
        string endpointKey,
        IReadOnlyDictionary<string, string>? queryParams = null,
        CancellationToken cancellationToken = default)
    {
        if (_options.UseMockResponses)
        {
            _sessionReady = true;
            return JsonSerializer.Serialize(new { mock = true, endpointKey });
        }

        await EnsureSessionAsync(cancellationToken).ConfigureAwait(false);

        if (!_routes.Routes.TryGetValue(endpointKey, out var relativePath) || string.IsNullOrWhiteSpace(relativePath))
        {
            _logger.LogWarning("Endpoint SAP não configurado para a chave {Key}", endpointKey);
            return null;
        }

        var url = relativePath.TrimStart('/');
        if (queryParams is { Count: > 0 })
        {
            var qs = string.Join("&", queryParams.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
            url += (url.Contains('?', StringComparison.Ordinal) ? "&" : "?") + qs;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _sessionReady = false;
            await EnsureSessionAsync(cancellationToken).ConfigureAwait(false);
            using var retry = new HttpRequestMessage(HttpMethod.Get, url);
            retry.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = await _http.SendAsync(retry, cancellationToken).ConfigureAwait(false);
        }

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogError("Erro ao consultar SAP ({Path}): {Status} {Body}", url, response.StatusCode, err);
            return null;
        }

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}
