using LotusWebApp.Data;
using LotusWebApp.Data.Models.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Services;

public interface IDeliveryService
{
    public Task<bool> Check(Guid orderId, int subscriptionId);

    public Task<bool> Accept(Guid orderId);
}

public class DeliveryService(
    ITopicProducer<string, DeliveryValidationResponseEvent> producer,
    ApplicationDbContext context,
    INotificationService notificationService): IDeliveryService
{
    public async Task<bool> Check(Guid orderId, int subscriptionId)
    {
        var checkResult = subscriptionId is 2 or 1;

        var order = await context.Orders
            .FirstOrDefaultAsync(x => x.Status == "Reserved" && x.Id == orderId);

        if (order == null)
        {
            return false;
        }

        if (checkResult)
        {
            order.Status = "DeliveryAccepted";
            await context.SaveChangesAsync();

            await notificationService.UserNotification(order.UserId, $"Hello! Your order {orderId} successfully sent to delivery", CancellationToken.None);

            var response = new DeliveryValidationResponseEvent
            {
                DeliveryAvailable = true,
                OrderId = orderId
            };

            await producer
                .Produce(Guid.NewGuid().ToString(), response)
                .ConfigureAwait(false);

            return true;
        }

        return false;
    }

    public async Task<bool> Accept(Guid orderId)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(x => x.Status == "Reserved" && x.Id == orderId);

        if (order == null)
        {
            return false;
        }

        order.Status = "DeliveryAccepted";
        await context.SaveChangesAsync();
        await notificationService.UserNotification(order.UserId, $"Hello! Your order {orderId} successfully sent to delivery", CancellationToken.None);
        var response = new DeliveryValidationResponseEvent
        {
            DeliveryAvailable = true,
            OrderId = orderId
        };

        await producer
            .Produce(Guid.NewGuid().ToString(), response)
            .ConfigureAwait(false);

        return true;
    }
}