using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuintasApp.Application.Features.Usuarios.Commands.ActualizarPerfil;
using QuintasApp.Application.Features.Usuarios.Commands.SyncFavoritos;
using QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;
using QuintasApp.Application.Features.Usuarios.Queries.GetFavoritos;
using QuintasApp.Application.Features.Usuarios.Queries.GetPerfil;
using System.Security.Claims;

namespace QuintasApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController(IMediator mediator) : ControllerBase
{
    private string SupabaseId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!;

    private string UserEmail => User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email") ?? "";

    private string UserName => User.FindFirstValue("name")
        ?? User.FindFirstValue(ClaimTypes.Name) ?? "";

    [HttpPost]
    public async Task<IActionResult> Upsert(CancellationToken ct)
    {
        var dto = await mediator.Send(new UpsertUsuarioCommand(SupabaseId, UserEmail, UserName), ct);
        return Ok(dto);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var dto = await mediator.Send(new GetPerfilQuery(SupabaseId), ct);
        return Ok(dto);
    }

    [HttpPatch("me")]
    public async Task<IActionResult> PatchMe([FromBody] ActualizarPerfilRequest req, CancellationToken ct)
    {
        var dto = await mediator.Send(new ActualizarPerfilCommand(SupabaseId, req.Nombre, req.Telefono), ct);
        return Ok(dto);
    }

    [HttpGet("me/favoritos")]
    public async Task<IActionResult> GetFavoritos(CancellationToken ct)
    {
        var ids = await mediator.Send(new GetFavoritosQuery(SupabaseId), ct);
        return Ok(ids);
    }

    [HttpPost("me/favoritos/sync")]
    public async Task<IActionResult> SyncFavoritos([FromBody] SyncFavoritosRequest req, CancellationToken ct)
    {
        var ids = await mediator.Send(new SyncFavoritosCommand(SupabaseId, req.QuintaIds), ct);
        return Ok(ids);
    }
}

public record ActualizarPerfilRequest(string? Nombre, string? Telefono);
public record SyncFavoritosRequest(List<string> QuintaIds);
