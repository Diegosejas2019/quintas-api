using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuintasApp.Application.Features.Conversaciones.Commands.EnviarMensaje;
using QuintasApp.Application.Features.Conversaciones.Commands.IniciarConversacion;
using QuintasApp.Application.Features.Conversaciones.Commands.MarcarLeida;
using QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByCliente;
using QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByQuinta;
using QuintasApp.Application.Features.Conversaciones.Queries.GetMensajes;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Services;
using static QuintasApp.Infrastructure.Services.ChatHub;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversacionesController(IMediator mediator, IConfiguration configuration, ChatHub chatHub) : ControllerBase
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

    [HttpGet("{id:guid}/stream")]
    [AllowAnonymous]
    public async Task StreamConversacion(Guid id, [FromQuery] string token, CancellationToken ct)
    {
        // Validar JWT manualmente (EventSource no soporta headers)
        var supabaseUrl = configuration["Supabase:Url"]!;
        var handler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            SignatureValidator = (t, _) => handler.ReadJwtToken(t),
        };

        ClaimsPrincipal principal;
        try
        {
            principal = handler.ValidateToken(token, validationParams, out _);
        }
        catch
        {
            Response.StatusCode = 401;
            return;
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? principal.FindFirstValue("sub");
        var tipoUsuario = principal.FindFirstValue("user_metadata.tipoUsuario")
                          ?? principal.FindFirstValue("tipoUsuario");
        var rol = tipoUsuario == "propietario" ? RemitenteRol.Propietario : RemitenteRol.Cliente;

        // Verificar acceso reutilizando GetMensajes como guard de autorización
        try
        {
            await mediator.Send(new GetMensajesQuery(id, userId!, rol), ct);
        }
        catch (UnauthorizedAccessException)
        {
            Response.StatusCode = 403;
            return;
        }
        catch
        {
            Response.StatusCode = 404;
            return;
        }

        // Configurar SSE
        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        ISseWriter writer = new SseClient(Response);
        chatHub.Suscribir(id, writer);

        try
        {
            await Task.Delay(Timeout.Infinite, ct);
        }
        catch (OperationCanceledException) { }
        finally
        {
            chatHub.Desuscribir(id, writer);
        }
    }
}

public record IniciarConversacionRequest(Guid QuintaId);
public record EnviarMensajeRequest(string Texto);
