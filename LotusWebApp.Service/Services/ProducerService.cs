using Confluent.Kafka;

namespace LotusWebApp.Services;

public class ProducerService
{
    private readonly IProducer<Null, string> _producer;

    public ProducerService(IConfiguration configuration)
    {
        var kafkaConnectionString = configuration
            .GetConnectionString("kafka.lotus") ?? "localhost:9092";
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaConnectionString
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task ProduceAsync(string message)
    {
        var kafkaMessage = new Message<Null, string> { Value = message, };

        await _producer.ProduceAsync("UserNotifications", kafkaMessage);
    }
}