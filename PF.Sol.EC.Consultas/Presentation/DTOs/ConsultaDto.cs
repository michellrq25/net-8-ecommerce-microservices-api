namespace PF.Sol.EC.Consultas.Presentation.DTOs
{
    public class ConsultaDto
    {
        public string? Id { get; set; }
        public int IdConsulta { get; set; }
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public PagoDto Pago { get; set; } = new();
    }

    public class CreateConsultaDto
    {
        public int IdConsulta { get; set; }
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }

    public class UpdateConsultaDto
    {

    }

    public class PagoDto
    {
        public int IdPago { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
