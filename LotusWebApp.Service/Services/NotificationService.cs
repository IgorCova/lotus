using Confluent.Kafka;
using LotusWebApp.Data;
using LotusWebApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LotusWebApp.Services;

public interface INotificationService
{
    Task ProduceAsync(string message);

    Task UserNotification(string userId, string message, CancellationToken cancellationToken);

    Task<List<Notification>> GetUserNotifications(string userId, CancellationToken cancellationToken);
}

public class NotificationService: INotificationService
{
    private readonly IProducer<Null, string> _producer;
    private readonly ApplicationDbContext _dbContext;

    public NotificationService(IConfiguration configuration, ApplicationDbContext context)
    {
        var kafkaConnectionString = configuration
            .GetConnectionString("kafka.lotus") ?? "localhost:9092";
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaConnectionString
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _dbContext = context;
    }

    public async Task ProduceAsync(string message)
    {
        var kafkaMessage = new Message<Null, string> { Value = message, };

        await _producer.ProduceAsync("UserNotifications", kafkaMessage);
    }

    public async Task UserNotification(string userId, string message, CancellationToken cancellationToken)
    {
        _dbContext.Notifications.Add(new Notification
        {
            UserId = userId,
            Message = message,
            Date = DateTime.Now
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
        var kafkaMessage = new
        {
            UserId = userId,
            Message = message
        };
        await ProduceAsync(JsonConvert.SerializeObject(kafkaMessage));
    }

    public async Task<List<Notification>> GetUserNotifications(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);
    }
}