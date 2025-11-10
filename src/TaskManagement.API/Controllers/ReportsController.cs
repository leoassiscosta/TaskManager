using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService reportService,
        ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Gera relatório de performance dos últimos 30 dias
    /// (Acesso restrito a usuários com role Manager)
    /// </summary>
    /// <param name="requestingUserId">ID do usuário solicitante</param>
    /// <returns>Relatório de performance</returns>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(PerformanceReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PerformanceReportResponse>> GetPerformanceReport(
        [FromQuery] Guid requestingUserId)
    {
        _logger.LogInformation("Generating performance report for user {UserId}", requestingUserId);
        var report = await _reportService.GetPerformanceReportAsync(requestingUserId);
        return Ok(report);
    }
}
