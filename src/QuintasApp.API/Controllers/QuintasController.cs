using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Quintas.Commands.CreateQuinta;
using QuintasApp.Application.Features.Quintas.Commands.DeleteQuinta;
using QuintasApp.Application.Features.Quintas.Commands.UpdateQuinta;
using QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuintasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetQuintasQuery(), ct));

    /// <summary>
    /// Devuelve solo las quintas disponibles este fin de semana (viernes→domingo próximos).
    /// Filtros opcionales: capacidad, precioMax, pileta, parrilla.
    /// </summary>
    [HttpGet("este-finde")]
    public async Task<IActionResult> GetEsteFinde(
        [FromQuery] int? capacidad,
        [FromQuery] decimal? precioMax,
        [FromQuery] bool? pileta,
        [FromQuery] bool? parrilla,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetEstefindeQuery(capacidad, precioMax, pileta, parrilla), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetQuintaByIdQuery(id), ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuintaCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuintaRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateQuintaCommand(id, req.Nombre, req.Descripcion, req.PrecioPorDia, req.Capacidad, req.Imagenes, req.Direccion, req.Pileta, req.Parrilla), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteQuintaCommand(id), ct);
        return NoContent();
    }
}

public record UpdateQuintaRequest(string Nombre, string? Descripcion, decimal PrecioPorDia, int Capacidad, List<string>? Imagenes, string? Direccion = null, bool Pileta = false, bool Parrilla = false);
