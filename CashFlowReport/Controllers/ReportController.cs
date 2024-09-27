using CashFlowInfra.Interfaces;
using CashFlowInfra.Models;
using Microsoft.AspNetCore.Mvc;

namespace CashFlowReport.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactionReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest("Start date and end date are required.");
        }

        var transactions = await _reportService.GenerateTransactionReport(startDate.Value, endDate.Value);
        return Ok(transactions);
    }

    [HttpGet("transactions/daily-balance")]
    public async Task<IActionResult> GetDailyBalanceReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest("Start date and end date are required.");
        }

        var transactions = await _reportService.GenerateDailyBalanceReport(startDate.Value, endDate.Value);
        return Ok(transactions);
    }
}