using LWMS.Application.Dashboard.Queries.GetKpi;
using LWMS.Application.Parcels.Queries.GetSlaAlert;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Controllers;

[Authorize(Roles = "Admin,HubManager")]
[Route("api/v1/dashboard")]
public class DashboardController : ApiControllerBase
{
    [HttpGet("kpi")]
    public async Task<ActionResult<KpiDto>> GetKpi()
    {
        return await Mediator.Send(new GetKpiQuery());
    }

    [HttpGet("sla-alerts")]
    public async Task<ActionResult<List<SlaAlertDto>>> GetSlaAlerts([FromQuery] int warningHours = 24)
    {
        return await Mediator.Send(new GetSlaAlertQuery { WarningHours = warningHours });
    }
}
