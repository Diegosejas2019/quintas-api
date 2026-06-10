using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Reservas.Commands.CancelReserva;
using QuintasApp.Application.Features.Reservas.Commands.CreateReserva;
using QuintasApp.Application.Features.Reservas.Commands.FinalizarReserva;
using QuintasApp.Application.Features.Reservas.Queries.GetDisponibilidad;
using QuintasApp.Application.Features.Reservas.Queries.GetReservas;
using QuintasApp.Application.Features.Senas.Commands.RegistrarSena;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] EstadoReserva? estado,
        [FromQuery] Guid? quintaId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetReservasQuery(estado, quintaId, page, size), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservaCommand cmd, CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);
        return Created($"api/reservas/{id}", new { id });
    }

    [HttpPut("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new CancelReservaCommand(id), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/finalizar")]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new FinalizarReservaCommand(id), ct);
        return NoContent();
    }

    [HttpGet("disponibilidad")]
    public async Task<IActionResult> Disponibilidad(
        [FromQuery] Guid quintaId,
        [FromQuery] int mes,
        [FromQuery] int anio,
        CancellationToken ct) =>
        Ok(await mediator.Send(new GetDisponibilidadQuery(quintaId, mes, anio), ct));

    [HttpPost("{id:guid}/sena")]
    public async Task<IActionResult> RegistrarSena(Guid id, [FromBody] RegistrarSenaRequest req, CancellationToken ct)
    {
        var senaId = await mediator.Send(
            new RegistrarSenaCommand(id, req.Monto, req.Tipo, req.Porcentaje, req.FechaPago, req.MetodoPago), ct);
        return Created($"api/reservas/{id}/sena", new { senaId });
    }
}

public record RegistrarSenaRequest(decimal Monto, TipoSena Tipo, decimal? Porcentaje, DateOnly FechaPago, string MetodoPago);
