namespace PF.Sol.EC.Pagos.Presentation.DTOs
{
    public class PagoDto
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }

    public class CreatePagoDto
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
