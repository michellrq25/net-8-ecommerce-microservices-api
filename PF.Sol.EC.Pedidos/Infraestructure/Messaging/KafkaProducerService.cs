using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PF.Sol.EC.Pedidos.Infraestructure.Configurations;
using PF.Sol.EC.Pedidos.Presentation.DTOs;
using PF.Sol.EC.Pedidos.Application.Interfaces;
using System.Text.Json;

namespace PF.Sol.EC.Pedidos.Infraestructure.Messaging
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IOptions<KafkaConfig> kafkaConfig;
        private readonly ILogger<KafkaProducerService> logger;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IOptions<KafkaConfig> kafkaConfig, ILogger<KafkaProducerService> logger)
        {
            this.kafkaConfig = kafkaConfig;
            this.logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaConfig.Value.BootstrapServers,
                ClientId = kafkaConfig.Value.ClientId,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task<bool> SendMessageAsync(PagoDto message, CancellationToken cancellationToken = default)
        {
            var messageJson = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = message.IdPago.ToString(),
                Value = messageJson,
                Timestamp = Timestamp.Default
            };

            try
            {
                var result = await _producer.ProduceAsync(kafkaConfig.Value.TopicName, kafkaMessage, cancellationToken);

                logger.LogInformation("Mensaje enviado a Kafka");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar el mensaje");
                return false;
            }
        }
    }
}
