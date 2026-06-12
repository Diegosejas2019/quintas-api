using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Conversaciones.Commands.EnviarMensaje;
using QuintasApp.Application.Features.Conversaciones.Commands.IniciarConversacion;
using QuintasApp.Application.Features.Conversaciones.Commands.MarcarLeida;
using QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByCliente;
using QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByQuinta;
using QuintasApp.Application.Features.Conversaciones.Queries.GetMensajes;
using QuintasApp.Domain.Enums;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversacionesController(IMediator mediator) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!;

    private string? TipoUsuario =>
        User.FindFirstValue("user_metadata.tipoUsuario")
        ?? User.FindFirstValue("tipoUsuario");

    private RemitenteRol RolActual =>
        TipoUsuario == "propietario" ? RemitenteRol.Propietario : RemitenteRol.Cliente;

    [HttpPost]
    public async Task<IActionResult> IniciarConversacion([FromBody] IniciarConversacionRequest req, CancellationToken ct)
    {
        if (RolActual == RemitenteRol.Propietario)
            return Forbid();

        var nombre = User.FindFirstValue("name") ?? User.FindFirstValue(ClaimTypes.Name) ?? "Cliente";
        var dto = await mediator.Send(new IniciarConversacionCommand(req.QuintaId, UserId, nombre), ct);
        return dto.TotalMensajes == 0
            ? CreatedAtAction(nameof(GetMensajes), new { id = dto.Id }, dto)
            : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetByQuinta([FromQuery] Guid quintaId, CancellationToken ct)
    {
        var lista = await mediator.Send(new GetConversacionesByQuintaQuery(quintaId, UserId), ct);
        return Ok(lista);
    }

    [HttpGet("mias")]
    public async Task<IActionResult> GetMias(CancellationToken ct)
    {
        var lista = await mediator.Send(new GetConversacionesByClienteQuery(UserId), ct);
        return Ok(lista);
    }

    [HttpGet("{id:guid}/mensajes")]
    public async Task<IActionResult> GetMensajes(Guid id, CancellationToken ct)
    {
        var mensajes = await mediator.Send(new GetMensajesQuery(id, UserId, RolActual), ct);
        return Ok(mensajes);
    }

    [HttpPost("{id:guid}/mensajes")]
    public async Task<IActionResult> EnviarMensaje(Guid id, [FromBody] EnviarMensajeRequest req, CancellationToken ct)
    {
        var dto = await mediator.Send(new EnviarMensajeCommand(id, UserId, RolActual, req.Texto), ct);
        return Created(string.Empty, dto);
    }

    [HttpPatch("{id:guid}/leer")]
    public async Task<IActionResult> MarcarLeida(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarcarLeidaCommand(id, UserId, RolActual), ct);
        return Ok();
    }
}

public record IniciarConversacionRequest(Guid QuintaId);
public record EnviarMensajeRequest(string Texto);
