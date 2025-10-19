using PF.Sol.EC.Pagos.Presentation.DTOs;

namespace PF.Sol.EC.Pagos.Application.Interfaces
{
    public interface IPagosService
    {
        Task<IEnumerable<PagoDto>> GetPagosAsync();
        Task<PagoDto> ProcesaPagoAsync(CreatePagoDto createPagoDto);
    }
}
