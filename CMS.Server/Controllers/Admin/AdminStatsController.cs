using CMS.Contracts.Admin.Dashboard;
using CMS.Server.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/stats")]
public sealed class AdminStatsController : ControllerBase
{
    private readonly IStatQueryService _statQueryService;

    public AdminStatsController(IStatQueryService statQueryService)
    {
        _statQueryService = statQueryService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats(
        CancellationToken cancellationToken)
    {
        var stats = await _statQueryService.GetDashboardStatsAsync(cancellationToken);
        return Ok(stats);
    }
}