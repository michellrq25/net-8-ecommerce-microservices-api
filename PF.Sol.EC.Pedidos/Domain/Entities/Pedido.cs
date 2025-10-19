namespace PF.Sol.EC.Pedidos.Domain.Entities
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public int IdCliente { get; set; }
        public decimal MontoPedido { get; set; }
        public int FormaPago { get; set; }
    }
}