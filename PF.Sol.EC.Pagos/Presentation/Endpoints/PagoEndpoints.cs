using PF.Sol.EC.Pagos.Application.Interfaces;
using PF.Sol.EC.Pagos.Presentation.DTOs;

namespace PF.Sol.EC.Pagos.Presentation.Endpoints
{
    public static class PagoEndpoints
    {
        public static void MapPagoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/pagos").WithTags("Pagos");

            // GET /api/pagos
            group.MapGet("/", async(IPagosService service) =>
            {
                var pagos = await service.GetPagosAsync();

                if (pagos == null || !pagos.Any())
                    return Results.NotFound("No se encontraron pagos");

                return Results.Ok(pagos);
            })
                .WithName("GetPagos")
                .WithOpenApi(); ;

            // POST /api/pagos/pago
            group.MapPost("/pago", async (CreatePagoDto createPagoDto, IPagosService service) =>
            {
                var result = await service.ProcesaPagoAsync(createPagoDto);

                if (result == null)
                {
                    // Si el servicio devolvió null, algo falló
                    return Results.Problem("No se pudo procesar el pago", statusCode: 500);
                }

                // Retorna 201 Created con la ubicación del recurso y el DTO
                return Results.Created($"/pago/{result.IdPago}", result);
            })
                .WithName("ProcesaPago")
                .WithOpenApi();
        }
    }
}
