using MongoDB.Driver;
using PF.Sol.EC.Consultas.Domain.Entities;

namespace PF.Sol.EC.Consultas.Infraestructure.Data
{
    public class ConsultasDbContext
    {
        private readonly IMongoDatabase _database;

        public ConsultasDbContext(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<Consulta> Consultas => _database.GetCollection<Consulta>("Consultas");
        public IMongoCollection<Counter> Counters => _database.GetCollection<Counter>("Counters");

        // Método para generar IdPago incremental
        public async Task<int> GetNextIdPagoAsync()
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, "Consulta");
            var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);

            var options = new FindOneAndUpdateOptions<Counter>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var counter = await Counters.FindOneAndUpdateAsync(filter, update, options);
            return counter.SequenceValue;
        }
    }
    public class Counter
    {
        public string Id { get; set; } = default!;
        public int SequenceValue { get; set; }
    }
}
