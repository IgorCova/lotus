using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Services;
using MassTransit;

namespace LotusWebApp.Consumers;

public sealed class StockValidationConsumer(IStockService stockService)
    : IConsumer<StockValidationRequestEvent>
{
    async Task IConsumer<StockValidationRequestEvent>
        .Consume(ConsumeContext<StockValidationRequestEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        var subscriptionId = int.TryParse(context.Message.SubscriptionId, out var subId) ? subId : 1;
        await stockService.Reserve(context.Message.OrderId, subscriptionId);
    }
}