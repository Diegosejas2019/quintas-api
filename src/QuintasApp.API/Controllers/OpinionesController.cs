using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Opiniones.Commands.CrearOpinion;
using QuintasApp.Application.Features.Opiniones.Queries.GetOpinionesByQuinta;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpinionesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{quintaId:guid}")]
    public async Task<IActionResult> GetByQuinta(Guid quintaId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetOpinionesByQuintaQuery(quintaId), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CrearOpinionRequest req, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")!;
        var id = await mediator.Send(new CrearOpinionCommand(req.QuintaId, userId, req.NombreCliente, req.Calificacion, req.Comentario), ct);
        return CreatedAtAction(nameof(GetByQuinta), new { quintaId = req.QuintaId }, new { id });
    }
}

public record CrearOpinionRequest(Guid QuintaId, string NombreCliente, int Calificacion, string? Comentario);
