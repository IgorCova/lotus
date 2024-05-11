using Confluent.Kafka;

namespace LotusWebApp.Worker;

public class ConsumerService : IHostedService
{
    private readonly ILogger<ConsumerService> _logger;

    public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var conf = new ConsumerConfig
        {
            GroupId = "LotusConsumerGroup",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var builder = new ConsumerBuilder<Null, string>(conf).Build();
        builder.Subscribe("UserNotifications");
        var cancelToken = new CancellationTokenSource();
        try
        {
            while (true)
            {
                var consumer = builder.Consume(cancelToken.Token);
                var message = consumer.Message.Value;
                _logger.LogInformation("Received user notifications: {Message}", message);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Error processing Kafka message: {ExMessage}", e.Message);
            builder.Close();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}