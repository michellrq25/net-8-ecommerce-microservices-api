using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PF.Sol.EC.Consultas.Domain.Entities
{
    public class Consulta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int IdConsulta { get; set; }
        public int IdPedido { get; set; }
        public string? NombreCliente { get; set; }
        public int IdPago { get; set; }
        public int FormaPago { get; set; } // 1 = Efectivo, 2 = TDC, 3 = TDD
        public decimal MontoPago { get; set; }
    }
}
