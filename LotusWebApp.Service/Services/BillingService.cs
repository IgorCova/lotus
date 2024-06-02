using LotusWebApp.Data;
using LotusWebApp.Data.Entities;
using LotusWebApp.Data.Models.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LotusWebApp.Services;

public interface IBillingService
{
    Task UserRegistered(string userId, CancellationToken cancellationToken);
    Task<bool> UserAddMoney(string userId, decimal money, CancellationToken cancellationToken);
    Task<bool> UserWithdrawsMoney(string userId, decimal money, CancellationToken cancellationToken);
    Task<bool> UserPayOrder(string userId, decimal money, CancellationToken cancellationToken);
    Task<bool> UserPayOrder(Guid orderId, CancellationToken cancellationToken);
    Task<decimal> UserBalance(string userId, CancellationToken cancellationToken);
}

public class BillingService(ApplicationDbContext context, INotificationService notificationService, ITopicProducer<string, BillingValidationResponseEvent> producer): IBillingService
{
    public async Task UserRegistered(string userId, CancellationToken cancellationToken)
    {
        context.Billings.Add(new Billing
        {
            Balance = 0,
            UserId = userId
        });
        await context.SaveChangesAsync(cancellationToken);

        context.BillingHistories.Add(new BillingHistory
        {
            BalanceBefore = 0,
            BalanceAfter = 0,
            UserId = userId,
            EventId = "UserRegistered"
        });
        await context.SaveChangesAsync(cancellationToken);
        await notificationService.UserNotification(userId, "Hello, You successfully registered", cancellationToken);
    }

    public async Task<bool> UserAddMoney(string userId, decimal money, CancellationToken cancellationToken)
    {
        var billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        if (billing == null)
        {
            await UserRegistered(userId, cancellationToken);
            billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        }

        if (billing == null)
        {
            throw new Exception("Billing for user not created");
        }

        var balanceBefore = billing.Balance;

        billing.Balance += money;
        await context.SaveChangesAsync(cancellationToken);

        context.BillingHistories.Add(new BillingHistory
        {
            BalanceBefore = balanceBefore,
            BalanceAfter = billing.Balance,
            UserId = userId,
            EventId = "UserAddedMoney"
        });
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.UserNotification(userId, $"Hello! You successfully added money amount {money}. You balance {billing.Balance}", cancellationToken);

        var userOrders = await context.Orders
            .Where(x => x.UserId == userId && x.Status == "Created")
            .ToListAsync(cancellationToken);

        if (userOrders.Count != 0)
        {
            foreach (var order in userOrders)
            {
                await UserPayOrder(order.Id, cancellationToken);
            }
        }

        return true;
    }

    public async Task<bool> UserWithdrawsMoney(string userId, decimal money, CancellationToken cancellationToken)
    {
        var billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        if (billing == null)
        {
            await UserRegistered(userId, cancellationToken);
            billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        }

        if (billing == null)
        {
            throw new Exception("Billing for user not created");
        }

        if (billing.Balance < money)
        {
            await notificationService.UserNotification(userId, $"Hello! You can not withdraws money amount {money}. Not enough money. You balance {billing.Balance}", cancellationToken);
            return false;
        }

        var balanceBefore = billing.Balance;

        billing.Balance -= money;
        await context.SaveChangesAsync(cancellationToken);

        context.BillingHistories.Add(new BillingHistory
        {
            BalanceBefore = balanceBefore,
            BalanceAfter = billing.Balance,
            UserId = userId,
            EventId = "UserWithdrawsMoney"
        });
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.UserNotification(userId, $"Hello! You successfully withdraws money amount {money}. You balance {billing.Balance}", cancellationToken);
        return true;
    }

    public async Task<bool> UserPayOrder(string userId, decimal money, CancellationToken cancellationToken)
    {
        var billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        if (billing == null)
        {
            await UserRegistered(userId, cancellationToken);
            billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        }

        if (billing == null)
        {
            throw new Exception("Billing for user not created");
        }

        if (billing.Balance < money)
        {
            await notificationService.UserNotification(userId, $"Hello! You cannot pay for order. Order cost amount {money}. You balance {billing.Balance}", cancellationToken);
            return false;
        }

        var balanceBefore = billing.Balance;

        billing.Balance -= money;
        await context.SaveChangesAsync(cancellationToken);

        context.BillingHistories.Add(new BillingHistory
        {
            BalanceBefore = balanceBefore,
            BalanceAfter = billing.Balance,
            UserId = userId,
            EventId = "UserPayForOrder"
        });
        await context.SaveChangesAsync(cancellationToken);
        await notificationService.UserNotification(userId, $"Hello! You pay for order. Order cost amount {money}. You balance after {billing.Balance}", cancellationToken);
        return true;
    }

    public async Task<bool> UserPayOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        if (order == null)
        {
            return false;
        }

        var subscriptionId = order.SubsriptionId;
        var cost = subscriptionId switch
        {
            1 => 10,
            2 => 95,
            3 => 1200,
            _=> 0
        };
        var payed = await UserPayOrder(order.UserId, cost, cancellationToken);
        if (payed)
        {
            order.Status = "Payed";
            await context.SaveChangesAsync(cancellationToken);
            var key = Guid.NewGuid();

            var response = new BillingValidationResponseEvent
            {
                HasMoney = true,
                OrderId = order.Id
            };

            await producer
                .Produce(key.ToString(), response, cancellationToken)
                .ConfigureAwait(false);
        }

        return payed;
    }

    public async Task<decimal> UserBalance(string userId, CancellationToken cancellationToken)
    {
        var billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        if (billing == null)
        {
            await UserRegistered(userId, cancellationToken);
            billing = context.Billings.FirstOrDefault(x => x.UserId == userId);
        }

        if (billing == null)
        {
            throw new Exception("Billing for user not created");
        }

        return billing.Balance;
    }
}