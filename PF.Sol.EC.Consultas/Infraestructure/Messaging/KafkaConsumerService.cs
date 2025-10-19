
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PF.Sol.EC.Consultas.Application.Interfaces;
using PF.Sol.EC.Consultas.Infraestructure.Configurations;
using PF.Sol.EC.Consultas.Presentation.DTOs;
using System.Text.Json;

namespace PF.Sol.EC.Consultas.Infraestructure.Messaging
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IOptions<KafkaConfig> kafkaConfig;
        private readonly ILogger<KafkaConsumerService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumerService(IOptions<KafkaConfig> kafkaConfig, ILogger<KafkaConsumerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this.kafkaConfig = kafkaConfig;
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaConfig.Value.BootstrapServers,
                GroupId = kafkaConfig.Value.GroupId,
                ClientId = kafkaConfig.Value.ClientId,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = true
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(2000, stoppingToken);

            try
            {
                _consumer.Subscribe(kafkaConfig.Value.TopicName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumerResult = _consumer.Consume(stoppingToken);
                    if (consumerResult != null)
                    {
                        string json = consumerResult.Message.Value;
                        logger.LogInformation($"Consumi el topico. Dato: {json}");
                        var messageModel = JsonSerializer.Deserialize<CreateConsultaDto>(json);
                        if (messageModel != null)
                        {
                            using var scope = serviceScopeFactory.CreateScope();
                            var consultaService = scope.ServiceProvider.GetService<IConsultasService>();
                            if( consultaService != null)
                            {
                                var res = await consultaService.CreateConsultaAsync(messageModel);
                                if (res != null) 
                                {
                                    logger.LogInformation("Datos procesados");
                                }
                                else
                                {
                                    logger.LogError("Error al procesar o sincronizar datos");
                                }
                            }
                        }
                    }
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudo consumir el topico");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}
