namespace PF.Sol.EC.Pagos.Domain.Entities
{
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdCliente { get; set; }
        public int FormaPago { get; set; }
        public decimal MontoPago { get; set; }
    }
}
