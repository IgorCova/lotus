using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Services;
using MassTransit;

namespace LotusWebApp.Consumers;

/// <inheritdoc />
public sealed class DeliveryValidationConsumer(IDeliveryService deliveryService)
    : IConsumer<DeliveryValidationRequestEvent>
{
    async Task IConsumer<DeliveryValidationRequestEvent>
        .Consume(ConsumeContext<DeliveryValidationRequestEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        await deliveryService.Check(context.Message.OrderId, context.Message.SubscriptionId);
    }
}