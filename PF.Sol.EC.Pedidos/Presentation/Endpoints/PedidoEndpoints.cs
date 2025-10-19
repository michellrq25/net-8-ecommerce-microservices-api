using PF.Sol.EC.Pedidos.Application.Interfaces;
using PF.Sol.EC.Pedidos.Presentation.DTOs;

namespace PF.Sol.EC.Pedidos.Presentation.Endpoints
{
    public static class PedidoEndpoints
    {
        public static void MapPedidoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/pedidos").WithTags("Pedidos").RequireAuthorization();

            // POST /api/pedidos/procesa
            group.MapPost("/procesa", async (CreatePedidoDto createPedidoDto, IPedidosService service, IKafkaProducerService kafkaProducer) =>
            {
                try
                {
                    var pedido = await service.ProcesaPedidoAsync(createPedidoDto);
                    var pago = new PagoDto
                    {
                        IdPedido = pedido.IdPedido,
                        NombreCliente = pedido.NombreCliente,
                        IdPago = pedido.IdPago,
                        MontoPago = pedido.MontoPedido,
                        FormaPago = pedido.FormaPago
                    };

                    await kafkaProducer.SendMessageAsync(pago);
                    return Results.Created($"/api/pedidos/{pedido.IdPedido}", pedido);
                }
                catch (KeyNotFoundException ex)
                {
                    return Results.NotFound(new { error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
                catch (Exception)
                {
                    return Results.Problem("Ocurrió un error inesperado al procesar el pedido");
                }
            })
                .WithName("ProcesaPedido")
                .WithOpenApi();

            // GET /api/pedidos
            group.MapGet("/", async (IPedidosService service) =>
            {
                var pedidos = await service.GetPedidosAsync();

                if (pedidos == null || !pedidos.Any())
                    return Results.NotFound("No se encontraron pedidos");

                return Results.Ok(pedidos);
            })
                .WithName("GetPedidos")
                .WithOpenApi();

            // GET /api/pedidos/clientes
            group.MapGet("/clientes", async (IPedidosService service) =>
            {
                var clientes = await service.GetClientesAsync();

                if (clientes == null || !clientes.Any())
                    return Results.NotFound("No se encontraron clientes");

                return Results.Ok(clientes);
            })
                .WithName("GetClientes")
                .WithOpenApi();
        }
    }
}
