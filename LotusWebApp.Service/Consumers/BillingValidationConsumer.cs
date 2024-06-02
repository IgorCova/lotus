using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Services;
using MassTransit;

namespace LotusWebApp.Consumers;

/// <inheritdoc />
public sealed class BillingValidationConsumer(IBillingService billingService)
    : IConsumer<BillingValidationRequestEvent>
{
    async Task IConsumer<BillingValidationRequestEvent>
        .Consume(ConsumeContext<BillingValidationRequestEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        await billingService.UserPayOrder(context.Message.OrderId, CancellationToken.None);
    }
}