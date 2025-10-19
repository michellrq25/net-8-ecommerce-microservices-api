using MongoDB.Driver;
using PF.Sol.EC.Consultas.Application.Interfaces;
using PF.Sol.EC.Consultas.Domain.Entities;
using PF.Sol.EC.Consultas.Infraestructure.Data;
using PF.Sol.EC.Consultas.Presentation.DTOs;

namespace PF.Sol.EC.Consultas.Application.Services
{
    public class ConsultasService : IConsultasService
    {
        private readonly ConsultasDbContext _context;
        private readonly ILogger<ConsultasService> _logger;

        public ConsultasService(ConsultasDbContext context, ILogger<ConsultasService> logger)
        {
            this._context = context;
            this._logger = logger;
        }
        public async Task<ConsultaDto> CreateConsultaAsync(CreateConsultaDto createConsultaDto)
        {
            try
            {
                _logger.LogInformation($"Creando un pedido procesado con código: {createConsultaDto.IdPedido}");
                var nuevoIdConsulta = await _context.GetNextIdPagoAsync();

                var consulta = new Consulta
                {
                    IdConsulta = nuevoIdConsulta,
                    IdPedido = createConsultaDto.IdPedido,
                    NombreCliente = createConsultaDto.NombreCliente,
                    IdPago = createConsultaDto.IdPago,
                    FormaPago = createConsultaDto.FormaPago,
                    MontoPago = createConsultaDto.MontoPago
                };

                await _context.Consultas.InsertOneAsync(consulta);

                _logger.LogInformation("Consulta registrada exitosamente. IdConsulta: {IdConsulta}", consulta.IdConsulta);

                return MapToDto(consulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear consulta para pedido {IdPedido}", createConsultaDto.IdPedido);
                return null!;
            }
        }

        public Task<bool> DeletePedidoAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ConsultaDto?> GetPedidoByIdAsync(int idConsulta)
        {
            _logger.LogInformation("Getting pedido with IdConsulta: {idConsulta}", idConsulta);
            var pedido = await _context.Consultas.Find(c => c.IdConsulta == idConsulta).FirstOrDefaultAsync();
            return pedido != null ? MapToDto(pedido) : null;
        }

        public async Task<ConsultaDto?> GetPedidoByIdPedidoAsync(int idPedido)
        {
            _logger.LogInformation("Getting pedido with IdPedido: {idPedido}", idPedido);
            var pedido = await _context.Consultas.Find(c => c.IdPedido == idPedido).FirstOrDefaultAsync();
            return pedido != null ? MapToDto(pedido) : null;
        }

        public async Task<IEnumerable<ConsultaDto>> GetPedidosAsync()
        {
            _logger.LogInformation("Getting all pedidos from MongoDB");
            var pedidos = await _context.Consultas.Find(_ => true).ToListAsync();
            return pedidos.Select(MapToDto);
        }

        public Task<IEnumerable<ConsultaDto>> GetPedidosByClienteAsync(int idCliente)
        {
            throw new NotImplementedException();
        }

        public Task<ConsultaDto?> UpdatePedidoAsync(string id, UpdateConsultaDto updateComprobanteDto)
        {
            throw new NotImplementedException();
        }

        private static ConsultaDto MapToDto(Consulta consulta)
        {
            return new ConsultaDto
            {
                Id = consulta.Id,
                IdConsulta = consulta.IdConsulta,
                IdPedido = consulta.IdPedido,
                NombreCliente = consulta.NombreCliente!,
                Pago = new PagoDto
                {
                    IdPago = consulta.IdPago,
                    FormaPago = consulta.FormaPago,
                    MontoPago = consulta.MontoPago
                }
            };
        }
    }
}
