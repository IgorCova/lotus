using LotusWebApp.Data;
using LotusWebApp.Data.Models.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Services;

public interface IStockService
{
    public Task<bool> Reserve(Guid orderId, int subscriptionId);

    public Task<bool> Add(int subscriptionId, int count);
}

public class StockService(
    ITopicProducer<string, StockValidationResponseEvent> producer,
    ApplicationDbContext context,
    INotificationService notificationService)
    : IStockService
{
    public async Task<bool> Reserve(Guid orderId, int subscriptionId)
    {
        if (subscriptionId is 1)
        {
            var key = Guid.NewGuid();
            var response = new StockValidationResponseEvent
            {
                SubscriptionId = subscriptionId.ToString(),
                OrderId = orderId,
                StockAvailable = true
            };

            var order = await context.Orders
                .Where(x => x.Status == "Payed" && x.Id == orderId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return false;
            }

            order.Status = "Reserved";
            await context.SaveChangesAsync();
            await notificationService.UserNotification(order.UserId, $"Hello! Your order {orderId} successfully reserved", CancellationToken.None);
            // Sending the validation stock based on the OrderId
            await producer
                .Produce(key.ToString(), response)
                .ConfigureAwait(false);

            return true;
        }

        return false;
    }

    public async Task<bool> Add(int subscriptionId, int count)
    {
        var orders = await context.Orders
            .Where(x => x.Status == "Payed" && x.SubsriptionId == subscriptionId)
            .ToListAsync();

        if (orders.Count != 0)
        {
            foreach (var order in orders)
            {
                var key = Guid.NewGuid();
                var response = new StockValidationResponseEvent
                {
                    SubscriptionId = subscriptionId.ToString(),
                    OrderId = order.Id,
                    StockAvailable = true
                };

                order.Status = "Reserved";
                await context.SaveChangesAsync();
                await notificationService.UserNotification(order.UserId, $"Hello! Your order {order.Id} successfully reserved", CancellationToken.None);
                // Sending the validation stock based on the OrderId
                await producer
                    .Produce(key.ToString(), response)
                    .ConfigureAwait(false);
            }
        }
        return true;
    }
}