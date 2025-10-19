using Azure.Core;
using Microsoft.EntityFrameworkCore;
using PF.Sol.EC.Pedidos.Application.Http;
using PF.Sol.EC.Pedidos.Application.Interfaces;
using PF.Sol.EC.Pedidos.Domain.Entities;
using PF.Sol.EC.Pedidos.Infraestructure.Data;
using PF.Sol.EC.Pedidos.Presentation.DTOs;
using System.Diagnostics;
using System.Text.Json;

namespace PF.Sol.EC.Pedidos.Application.Services
{
    public class PedidosService : IPedidosService
    {
        private static readonly ActivitySource ActivitySource = new("ApiPedidos");

        private readonly PedidoDbContext _context;
        private readonly ILogger<PedidosService> _logger;
        private readonly IHttpClientService _httpClient;
        private readonly IConfiguration _configuration;

        public PedidosService(PedidoDbContext context, ILogger<PedidosService> logger, IHttpClientService httpClient, IConfiguration configuration)
        {
            this._context = context;
            this._logger = logger;
            this._httpClient = httpClient;
            this._configuration = configuration;
        }
        public async Task<ClienteDto> CreateClienteAsync(CreateClienteDto createClienteDto)
        {
            _logger.LogInformation("Creating new client: {NombreCliente}", createClienteDto.NombreCliente);
            var cliente = new Cliente
            {
                NombreCliente = createClienteDto.NombreCliente
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                IdCliente = cliente.IdCliente,
                NombreCliente = cliente.NombreCliente
            };
        }

        public async Task<PedidoDto> ProcesaPedidoAsync(CreatePedidoDto createPedidoDto)
        {
            _logger.LogInformation("Creando nuevo pedido para el cliente: {IdCliente}", createPedidoDto.IdCliente);

            // Validar cliente
            var cliente = await _context.Clientes.FindAsync(createPedidoDto.IdCliente)
                          ?? throw new KeyNotFoundException($"El cliente con Id {createPedidoDto.IdCliente} no existe");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Pedido pedido;
                var zonaLima = TimeZoneInfo.FindSystemTimeZoneById(_configuration["RegionalSettings:TimeZone"]!); // "SA Pacific Standard Time"

                // Span corto: PedidoBD
                using (var dbSpan = ActivitySource.StartActivity("GuardarPedidoBD"))
                {
                    pedido = new Pedido
                    {
                        IdCliente = createPedidoDto.IdCliente,
                        FechaPedido = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaLima),
                        MontoPedido = createPedidoDto.MontoPedido,
                        FormaPago = createPedidoDto.FormaPago
                    };

                    _context.Pedidos.Add(pedido);
                    await _context.SaveChangesAsync();

                    dbSpan?.SetTag("IdPedido", pedido.IdPedido);
                }

                // Procesar pago
                var pagoMongo = new PagoMongo
                {
                    IdCliente = pedido.IdCliente,
                    FormaPago = pedido.FormaPago,
                    MontoPago = pedido.MontoPedido
                };

                PagoDto pagoResponse = (PagoDto)await SendToPagoAsync(pagoMongo);

                if (pagoResponse == null)
                {
                    _logger.LogError("Fallo al procesar el pago para el pedido {IdPedido}", pedido.IdPedido);
                    throw new InvalidOperationException("No se pudo procesar el pago");
                }

                // Span: EnvioKafka
                using (var kafkaSpan = ActivitySource.StartActivity("EnvioKafka"))
                {
                    kafkaSpan?.SetTag("IdPago", pagoResponse.IdPago);
                    kafkaSpan?.SetTag("IdPedido", pedido.IdPedido);
                }

                    await transaction.CommitAsync();

                _logger.LogInformation("Pedido {IdPedido} procesado exitosamente", pedido.IdPedido);

                return new PedidoDto
                {
                    IdPedido = pedido.IdPedido,
                    FechaPedido = pedido.FechaPedido,
                    IdCliente = pedido.IdCliente,
                    MontoPedido = pedido.MontoPedido,
                    IdPago = pagoResponse.IdPago,
                    NombreCliente = cliente.NombreCliente,
                    FormaPago = pedido.FormaPago
                };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al procesar pedido para cliente {IdCliente}", createPedidoDto.IdCliente);
                throw;
            }            
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            _logger.LogInformation("Deleting client with ID: {Id}", id);
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePedidoAsync(int id)
        {
            _logger.LogInformation("Deleting pedido with ID: {Id}", id);
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return false;

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ClienteDto?> GetClienteByIdAsync(int id)
        {
            _logger.LogInformation("Getting client with ID: {Id}", id);
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                IdCliente = cliente.IdCliente,
                NombreCliente = cliente.NombreCliente
            };
        }

        public async Task<IEnumerable<ClienteDto>> GetClientesAsync()
        {
            _logger.LogInformation("Getting all clients");
            var clientes = await _context.Clientes.ToListAsync();
            return clientes.Select(c => new ClienteDto
            {
                IdCliente = c.IdCliente,
                NombreCliente = c.NombreCliente
            });
        }

        public async Task<PedidoDto?> GetPedidoByIdAsync(int id)
        {
            _logger.LogInformation("Getting pedido with ID: {Id}", id);
            var articulo = await _context.Pedidos.FindAsync(id);
            if (articulo == null) return null;

            return new PedidoDto
            {
                IdPedido = articulo.IdPedido,
                FechaPedido = articulo.FechaPedido,
                IdCliente = articulo.IdCliente,
                MontoPedido = articulo.MontoPedido
            };
        }

        public async Task<IEnumerable<PedidoDto>> GetPedidosAsync()
        {
            _logger.LogInformation("Getting all pedidos");
            var articulos = await _context.Pedidos.ToListAsync();
            return articulos.Select(a => new PedidoDto
            {
                IdPedido = a.IdPedido,
                FechaPedido = a.FechaPedido,
                IdCliente = a.IdCliente,
                MontoPedido = a.MontoPedido
            });
        }

        public async Task<ClienteDto?> UpdateClienteAsync(int id, UpdateClienteDto updateClienteDto)
        {
            _logger.LogInformation("Updating client with ID: {Id}", id);
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;

            cliente.NombreCliente = updateClienteDto.NombreCliente;
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                IdCliente = cliente.IdCliente,
                NombreCliente = cliente.NombreCliente
            };
        }

        public async Task<PedidoDto?> UpdatePedidoAsync(int id, PedidoDto updatePedidoDto)
        {
            _logger.LogInformation("Updating pedido with ID: {Id}", id);
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return null;

            pedido.FechaPedido = updatePedidoDto.FechaPedido;
            pedido.IdCliente = updatePedidoDto.IdCliente;
            pedido.MontoPedido = updatePedidoDto.MontoPedido;

            await _context.SaveChangesAsync();

            return await GetPedidoByIdAsync(id);
        }

        private async Task<object> SendToPagoAsync(PagoMongo pagoMongo)
        {
            var url = "/api/pagos/pago";

            var result = await _httpClient.PostAsync<PagoDto>(url, pagoMongo);
            return result!;
        }
    }
}
