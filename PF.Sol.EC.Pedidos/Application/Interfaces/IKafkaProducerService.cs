using PF.Sol.EC.Pedidos.Presentation.DTOs;

namespace PF.Sol.EC.Pedidos.Application.Interfaces
{
    public interface IKafkaProducerService
    {
        Task<bool> SendMessageAsync(PagoDto message, CancellationToken cancellationToken = default);
    }
}
