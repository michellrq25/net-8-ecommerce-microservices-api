using PF.Sol.EC.Consultas.Presentation.DTOs;

namespace PF.Sol.EC.Consultas.Application.Interfaces
{
    public interface IConsultasService
    {
        Task<IEnumerable<ConsultaDto>> GetPedidosAsync();
        Task<ConsultaDto?> GetPedidoByIdAsync(int idConsulta);
        Task<ConsultaDto?> GetPedidoByIdPedidoAsync(int idPedido);
        Task<ConsultaDto> CreateConsultaAsync(CreateConsultaDto createComprobanteDto);
        Task<ConsultaDto?> UpdatePedidoAsync(string id, UpdateConsultaDto updateComprobanteDto);
        Task<bool> DeletePedidoAsync(string id);
        Task<IEnumerable<ConsultaDto>> GetPedidosByClienteAsync(int idCliente);
    }
}
