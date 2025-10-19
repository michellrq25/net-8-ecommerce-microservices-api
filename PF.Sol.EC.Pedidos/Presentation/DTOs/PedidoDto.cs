using System.ComponentModel.DataAnnotations;

namespace PF.Sol.EC.Pedidos.Presentation.DTOs
{
    public class PedidoDto
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public int IdCliente { get; set; }
        public decimal MontoPedido { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public int IdPago { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string? NombreCliente { get; set; }
        public int FormaPago { get; set; }
    }

    public class CreatePedidoDto
    {     
        [Required]
        public int IdCliente { get; set; }
        [Required]
        public decimal MontoPedido { get; set; }
        [Required]
        public int FormaPago { get; set; }
    }

    public class UpdatePedidoDto
    {
        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        public int IdCliente { get; set; }
        [Required]
        public decimal MontoPedido { get; set; }
        public int FormaPago { get; set; }
    }
}
