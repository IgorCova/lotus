using LotusWebApp.Data.Models.Exceptions;
using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Saga.Services;
using MassTransit;

namespace LotusWebApp.Saga.StateMachines.CustomActivities;

public sealed class BillingValidationActivity(
    ITopicProducer<string, StockValidationRequestEvent> stockValidationEngineProducer,
    IApiService apiService)
    : IStateMachineActivity<OrderRequestSagaInstance, BillingValidationResponseEvent>
{
    async Task IStateMachineActivity<OrderRequestSagaInstance, BillingValidationResponseEvent>.Execute(
        BehaviorContext<OrderRequestSagaInstance, BillingValidationResponseEvent> context,
        IBehavior<OrderRequestSagaInstance, BillingValidationResponseEvent> next)
    {
        if (!await apiService.ValidateIncomingBillingResult(context.Message))
        {
            throw new NotATransientException("Error during the billing validation!");
        }

        context.Saga.NotificationReply = new NotificationReply<OrderResponseEvent>
        {
            Success = true,
            Data = new OrderResponseEvent
            {
                UserId = context.Saga.UserId!,
                CustomerType = context.Saga.CustomerType ?? "User",
                BillingValidation = context.Message
            }
        };
        
        var stockValidationRequestEvent = new StockValidationRequestEvent
        {
            SubscriptionId = context.Saga.SubscriptionId ?? string.Empty,
            OrderId = context.Saga.OrderId
        };
        
        await stockValidationEngineProducer
            .Produce(
                Guid.NewGuid().ToString(), 
                stockValidationRequestEvent,
                context.CancellationToken)
            .ConfigureAwait(false);
        
        await next
            .Execute(context)
            .ConfigureAwait(false);
    }

    async Task IStateMachineActivity<OrderRequestSagaInstance, BillingValidationResponseEvent>.Faulted<TException>(
        BehaviorExceptionContext<OrderRequestSagaInstance, BillingValidationResponseEvent, TException> context,
        IBehavior<OrderRequestSagaInstance, BillingValidationResponseEvent> next)
            => await next.Faulted(context).ConfigureAwait(false);

    void IProbeSite.Probe(ProbeContext context) => context.CreateScope(nameof(BillingValidationActivity));
    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}