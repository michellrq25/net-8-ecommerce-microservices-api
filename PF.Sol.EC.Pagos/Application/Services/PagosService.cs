using Microsoft.EntityFrameworkCore;
using PF.Sol.EC.Pagos.Application.Interfaces;
using PF.Sol.EC.Pagos.Domain.Entities;
using PF.Sol.EC.Pagos.Infraestructure.Data;
using PF.Sol.EC.Pagos.Presentation.DTOs;
using System.Diagnostics;

namespace PF.Sol.EC.Pagos.Application.Services
{
    public class PagosService : IPagosService
    {
        private static readonly ActivitySource ActivitySource = new("ApiPagos");

        private readonly PagosDbContext _context;
        private readonly ILogger<PagosService> _logger;
        private readonly IConfiguration _configuration;

        public PagosService(PagosDbContext context, ILogger<PagosService> logger, IConfiguration configuration)
        {
            this._context = context;
            this._logger = logger;
            this._configuration = configuration;
        }

        public async Task<IEnumerable<PagoDto>> GetPagosAsync()
        {
            _logger.LogInformation("Getting all pedidos");
            var pagos = await _context.Pagos
                              .ToListAsync();

            return pagos.Select(MapToDto);
        }

        public async Task<PagoDto> ProcesaPagoAsync(CreatePagoDto createPagoDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Realizando nuevo pago para el Cliente: {IdCliente}", createPagoDto.IdCliente);

                var zonaLima = TimeZoneInfo.FindSystemTimeZoneById(_configuration["RegionalSettings:TimeZone"]!); // "SA Pacific Standard Time"

                // Span: GuardarPagoBD
                using (var dbSpan = ActivitySource.StartActivity("GuardarPagoBD"))
                {
                    dbSpan?.SetTag("IdCliente", createPagoDto.IdCliente);

                    var pago = new Pago
                    {
                        FechaPago = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaLima),
                        IdCliente = createPagoDto.IdCliente,
                        FormaPago = createPagoDto.FormaPago,
                        MontoPago = createPagoDto.MontoPago
                    };

                    _context.Pagos.Add(pago);
                    await _context.SaveChangesAsync();

                    dbSpan?.SetTag("IdPago", pago.IdPago);

                    await transaction.CommitAsync();
                    _logger.LogInformation("Pago registrado exitosamente. IdPago: {IdPago}", pago.IdPago);

                    return MapToDto(pago);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error inesperado al realizar el pago para cliente {IdCliente}", createPagoDto.IdCliente);
                return null!;
            }
        }

        private static PagoDto MapToDto(Pago pago)
        {
            return new PagoDto
            {
                IdPago = pago.IdPago,
                FechaPago = pago.FechaPago,
                IdCliente = pago.IdCliente,
                FormaPago = pago.FormaPago,
                MontoPago = pago.MontoPago
            };
        }
    }
}
