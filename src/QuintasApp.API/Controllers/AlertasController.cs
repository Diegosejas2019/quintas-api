using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Alertas.Commands.CrearAlerta;
using QuintasApp.Application.Features.Alertas.Commands.EliminarAlerta;
using QuintasApp.Application.Features.Alertas.Queries.GetAlertasByUser;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertasController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!;

    [HttpGet]
    public async Task<IActionResult> GetMisAlertas(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAlertasByUserQuery(UserId), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearAlertaRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CrearAlertaCommand(UserId, req.QuintaId, req.FechaInicio, req.FechaFin, req.Email), ct);
        return CreatedAtAction(nameof(GetMisAlertas), new { id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new EliminarAlertaCommand(id, UserId), ct);
        return NoContent();
    }
}

public record CrearAlertaRequest(Guid QuintaId, DateOnly FechaInicio, DateOnly FechaFin, string Email);
