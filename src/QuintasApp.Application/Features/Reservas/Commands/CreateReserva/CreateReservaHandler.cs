using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaHandler(
    IQuintaRepository quintaRepo,
    IReservaRepository reservaRepo) : IRequestHandler<CreateReservaCommand, Guid>
{
    public async Task<Guid> Handle(CreateReservaCommand cmd, CancellationToken ct)
    {
        var quinta = await quintaRepo.GetByIdAsync(cmd.QuintaId, ct)
            ?? throw new DomainException($"Quinta con id {cmd.QuintaId} no encontrada.");

        if (!quinta.Activa)
            throw new DomainException("La quinta no está disponible para reservas.");

        var reserva = Reserva.Crear(
            cmd.QuintaId,
            cmd.NombreCliente,
            cmd.EmailCliente,
            cmd.TelefonoCliente,
            cmd.FechaInicio,
            cmd.FechaFin,
            quinta.PrecioPorDia);

        // InsertarFechasOcupadasAsync uses the unique (quintaId, fecha) index to atomically
        // detect overlaps. Throws FechasSuperposicionException on duplicate (409).
        await reservaRepo.InsertarFechasOcupadasAsync(reserva.Id, cmd.QuintaId, cmd.FechaInicio, cmd.FechaFin, ct);
        await reservaRepo.AddAsync(reserva, quinta.Nombre, ct);

        return reserva.Id;
    }
}
