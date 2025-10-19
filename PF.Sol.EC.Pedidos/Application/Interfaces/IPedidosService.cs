using PF.Sol.EC.Pedidos.Domain.Entities;
using PF.Sol.EC.Pedidos.Presentation.DTOs;

namespace PF.Sol.EC.Pedidos.Application.Interfaces
{
    public interface IPedidosService
    {
        // Cliente operations
        Task<IEnumerable<ClienteDto>> GetClientesAsync();
        Task<ClienteDto?> GetClienteByIdAsync(int id);
        Task<ClienteDto> CreateClienteAsync(CreateClienteDto createClienteDto);
        Task<ClienteDto?> UpdateClienteAsync(int id, UpdateClienteDto updateClienteDto);
        Task<bool> DeleteClienteAsync(int id);

        // Pedido operations
        Task<IEnumerable<PedidoDto>> GetPedidosAsync();
        Task<PedidoDto?> GetPedidoByIdAsync(int id);
        Task<PedidoDto> ProcesaPedidoAsync(CreatePedidoDto createPedidoDto);
        Task<PedidoDto?> UpdatePedidoAsync(int id, PedidoDto updatePedidoDto);
        Task<bool> DeletePedidoAsync(int id);
    }
}
