using LotusWebApp.Data;
using LotusWebApp.Data.Entities;
using LotusWebApp.Data.Models.Saga;
using MassTransit;

namespace LotusWebApp.Services;

public interface IOrderService
{
    Task<bool> MakeAnOrder(string userId, decimal amount, CancellationToken cancellationToken);
    Task<Guid> PlaceOrder(string userId, int subscriptionId, CancellationToken cancellationToken);
}

public class OrderService(IBillingService billingService, ApplicationDbContext dbContext, ITopicProducer<string, OrderRequestEvent> producer): IOrderService
{
    public async Task<bool> MakeAnOrder(string userId, decimal amount, CancellationToken cancellationToken)
    {
        return await billingService.UserPayOrder(userId, amount, cancellationToken);
    }

    public async Task<Guid> PlaceOrder(string userId, int subscriptionId, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SubsriptionId = subscriptionId,
            Status = "Created"
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        var key = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var orderRequest = new OrderRequestEvent
        {
            OrderId = order.Id,
            UserId = userId,
            SubscriptionId = subscriptionId.ToString()
        };

        await producer
            .Produce(
                key.ToString(),
                orderRequest,
                Pipe.Execute<KafkaSendContext>(p =>
                {
                    p.CorrelationId = correlationId;
                }), cancellationToken)
            .ConfigureAwait(false);

        return order.Id;
    }
}