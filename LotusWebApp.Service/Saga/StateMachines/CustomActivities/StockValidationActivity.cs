using LotusWebApp.Data.Models.Exceptions;
using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Saga.Services;
using MassTransit;

namespace LotusWebApp.Saga.StateMachines.CustomActivities;

/// <inheritdoc />
public sealed class StockValidationActivity(IApiService apiService,
    ITopicProducer<string, DeliveryValidationRequestEvent> deliveryValidationEngineProducer)
    : IStateMachineActivity<OrderRequestSagaInstance, StockValidationResponseEvent>
{
    async Task IStateMachineActivity<OrderRequestSagaInstance, StockValidationResponseEvent>.Execute(
        BehaviorContext<OrderRequestSagaInstance, StockValidationResponseEvent> context,
        IBehavior<OrderRequestSagaInstance, StockValidationResponseEvent> next)
    {
        if (!await apiService.ValidateIncomingStockResult(context.Message))
        {
            throw new NotATransientException("Error during order request validation!");
        }

        context.Saga.NotificationReply = new NotificationReply<OrderResponseEvent>
        {
            Success = true,
            Data = new OrderResponseEvent
            {
                UserId = context.Saga.UserId!,
                CustomerType = context.Saga.CustomerType ?? "User",
                StockValidation = context.Message
            }
        };

        var deliveryValidationRequestEvent = new DeliveryValidationRequestEvent
        {
            UserId = context.Saga.UserId ?? string.Empty,
            OrderId = context.Saga.OrderId,
            SubscriptionId = int.TryParse(context.Saga.SubscriptionId, out var subscriptionId) ? subscriptionId : 1
        };

        await deliveryValidationEngineProducer
            .Produce(
                Guid.NewGuid().ToString(),
                deliveryValidationRequestEvent,
                context.CancellationToken)
            .ConfigureAwait(false);

        await next
            .Execute(context)
            .ConfigureAwait(false);
    }

    async Task IStateMachineActivity<OrderRequestSagaInstance, StockValidationResponseEvent>.Faulted<TException>(
        BehaviorExceptionContext<OrderRequestSagaInstance, StockValidationResponseEvent, TException> context,
        IBehavior<OrderRequestSagaInstance, StockValidationResponseEvent> next)
            => await next.Faulted(context).ConfigureAwait(false);

    void IProbeSite.Probe(ProbeContext context) => context.CreateScope(nameof(ReceiveOrderRequestActivity));
    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}