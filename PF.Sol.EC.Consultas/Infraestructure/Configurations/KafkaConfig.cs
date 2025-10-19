namespace PF.Sol.EC.Consultas.Infraestructure.Configurations
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
    }
}
