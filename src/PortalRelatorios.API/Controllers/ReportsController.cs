using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalRelatorios.Application.Abstractions;

namespace PortalRelatorios.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly ISapServiceLayerService _sap;

    public ReportsController(ISapServiceLayerService sap)
    {
        _sap = sap;
    }

    /// <summary>Consulta genérica a um endpoint configurado no Service Layer (chave em ReportEndpoints:Routes).</summary>
    [HttpGet("sap/{endpointKey}")]
    public async Task<IActionResult> GetFromSap(string endpointKey, CancellationToken cancellationToken)
    {
        var json = await _sap.GetReportDataAsync(endpointKey, null, cancellationToken).ConfigureAwait(false);
        if (json is null)
            return NotFound();
        return Content(json, "application/json");
    }
}
