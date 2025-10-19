using System.ComponentModel.DataAnnotations;

namespace PF.Sol.EC.Pedidos.Presentation.DTOs
{
    public class ClienteDto
    {
        public int IdCliente { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreCliente { get; set; } = string.Empty;
    }

    public class CreateClienteDto
    {
        [Required]
        [MaxLength(100)]
        public string NombreCliente { get; set; } = string.Empty;
    }
    public class UpdateClienteDto
    {
        [Required]
        [MaxLength(100)]
        public string NombreCliente { get; set; } = string.Empty;
    }
}
