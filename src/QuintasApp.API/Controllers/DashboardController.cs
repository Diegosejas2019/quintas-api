using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Dashboard.Queries.GetDashboard;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(await mediator.Send(new GetDashboardQuery(), ct));
}
