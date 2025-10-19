using PF.Sol.EC.Consultas.Application.Interfaces;
using PF.Sol.EC.Consultas.Presentation.DTOs;

namespace PF.Sol.EC.Consultas.Presentation.Endpoints
{
    public static class ConsultaEndpoints
    {
        public static void MapConsultaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/consultas").WithTags("Consultas").RequireAuthorization();

            // GET /api/consultas
            group.MapGet("/", async (IConsultasService consultasService) =>
            {
                var pedidos = await consultasService.GetPedidosAsync();

                if (pedidos == null || !pedidos.Any())
                    return Results.NotFound("No se encontraron pedidos");

                return Results.Ok(pedidos);
            })
                .WithName("GetPedidos")
                .WithOpenApi();

            // GET /api/consultas/id
            group.MapGet("/{idConsulta}", async (int idConsulta, IConsultasService service) =>
            {
                var pedido = await service.GetPedidoByIdAsync(idConsulta);
                return pedido is not null ? Results.Ok(pedido) : Results.NotFound();
            })
                 .WithName("GetComprobante")
                 .WithOpenApi();

            // GET /api/consultas/pedido/idPedido
            group.MapGet("/pedido/{idPedido}", async (int idPedido, IConsultasService service) =>
            {
                var consulta = await service.GetPedidoByIdPedidoAsync(idPedido);
                return consulta is not null ? Results.Ok(consulta) : Results.NotFound();
            })
                .WithName("GetConsultaByIdPedido")
                .WithOpenApi();

            // POST /api/consultas/consulta
            group.MapPost("/consulta", async (CreateConsultaDto createConsultaDto, IConsultasService service) =>
            {
                var pedido = await service.CreateConsultaAsync(createConsultaDto);
                return Results.Created($"/api/consultas/{createConsultaDto.IdConsulta}", pedido);
            })
                .WithName("ProcesarPedidos")
                .WithOpenApi();

        }
    }
}
