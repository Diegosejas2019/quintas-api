using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Quintas.Commands.BloquearFechas;
using QuintasApp.Application.Features.Quintas.Commands.CreateQuinta;
using QuintasApp.Application.Features.Quintas.Commands.DeleteQuinta;
using QuintasApp.Application.Features.Quintas.Commands.UpdateQuinta;
using QuintasApp.Application.Features.Quintas.Queries.GetDisponibles;
using QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuintasController(IMediator mediator) : ControllerBase
{
    private string? SupabaseId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    private string? TipoUsuario => User.FindFirstValue("user_metadata.tipoUsuario")
        ?? User.FindFirstValue("tipoUsuario");

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var esPropietario = User.Identity?.IsAuthenticated == true && TipoUsuario == "propietario";
        var propietarioId = esPropietario ? SupabaseId : null;
        return Ok(await mediator.Send(new GetQuintasQuery(propietarioId), ct));
    }

    [HttpGet("este-finde")]
    public async Task<IActionResult> GetEsteFinde(
        [FromQuery] int? capacidad,
        [FromQuery] decimal? precioMax,
        [FromQuery] bool? pileta,
        [FromQuery] bool? parrilla,
        [FromQuery] DateOnly? fechaInicio,
        [FromQuery] DateOnly? fechaFin,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetEstefindeQuery(capacidad, precioMax, pileta, parrilla, fechaInicio, fechaFin), ct);
        return Ok(result);
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> GetDisponibles(
        [FromQuery] DateOnly fechaInicio,
        [FromQuery] DateOnly fechaFin,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetDisponiblesQuery(fechaInicio, fechaFin), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var esCliente = User.Identity?.IsAuthenticated == true && TipoUsuario != "propietario";
        var clienteId = esCliente ? SupabaseId : null;
        var result = await mediator.Send(new GetQuintaByIdQuery(id, clienteId), ct);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuintaRequest req, CancellationToken ct)
    {
        var cmd = new CreateQuintaCommand(req.Nombre, req.Descripcion, req.PrecioPorDia, req.Capacidad,
            req.Imagenes, req.Direccion, req.Pileta, req.Parrilla, req.Amenities, req.Latitud, req.Longitud,
            PropietarioId: SupabaseId!, HoraInicio: req.HoraInicio, HoraFin: req.HoraFin);
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuintaRequest req, CancellationToken ct)
    {
        await mediator.Send(new UpdateQuintaCommand(id, req.Nombre, req.Descripcion, req.PrecioPorDia,
            req.Capacidad, req.Imagenes, req.Direccion, req.Pileta, req.Parrilla, req.Amenities,
            req.Latitud, req.Longitud, PropietarioId: SupabaseId!, HoraInicio: req.HoraInicio, HoraFin: req.HoraFin), ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteQuintaCommand(id, PropietarioId: SupabaseId!), ct);
        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:guid}/bloqueos")]
    public async Task<IActionResult> BloquearFechas(Guid id, [FromBody] BloquearFechasRequest req, CancellationToken ct)
    {
        var fechas = req.Fechas.Select(f => DateOnly.ParseExact(f, "yyyy-MM-dd")).ToList();
        await mediator.Send(new BloquearFechasCommand(id, fechas, PropietarioId: SupabaseId!), ct);
        return NoContent();
    }
}

public record CreateQuintaRequest(
    string Nombre, string? Descripcion, decimal PrecioPorDia, int Capacidad,
    List<string>? Imagenes = null, string? Direccion = null,
    bool Pileta = false, bool Parrilla = false, List<string>? Amenities = null,
    decimal? Latitud = null, decimal? Longitud = null,
    string? HoraInicio = null, string? HoraFin = null);

public record UpdateQuintaRequest(
    string Nombre, string? Descripcion, decimal PrecioPorDia, int Capacidad,
    List<string>? Imagenes, string? Direccion = null,
    bool Pileta = false, bool Parrilla = false, List<string>? Amenities = null,
    decimal? Latitud = null, decimal? Longitud = null,
    string? HoraInicio = null, string? HoraFin = null);

public record BloquearFechasRequest(List<string> Fechas);
