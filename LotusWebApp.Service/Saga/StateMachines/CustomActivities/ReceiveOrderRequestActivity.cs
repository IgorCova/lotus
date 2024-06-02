using LotusWebApp.Data.Models.Exceptions;
using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Saga.Services;
using MassTransit;

namespace LotusWebApp.Saga.StateMachines.CustomActivities;

public sealed class ReceiveOrderRequestActivity(
    IApiService apiService,
    ITopicProducer<string, BillingValidationRequestEvent> billingValidationEngineProducer)
    : IStateMachineActivity<OrderRequestSagaInstance, OrderRequestEvent>
{
    async Task IStateMachineActivity<OrderRequestSagaInstance, OrderRequestEvent>.Execute(
        BehaviorContext<OrderRequestSagaInstance, OrderRequestEvent> context, 
        IBehavior<OrderRequestSagaInstance, OrderRequestEvent> next)
    {
        // Some dummy validation to test the error handling flow
        if (await apiService
            .ValidateIncomingOrderRequestAsync(context.Message)
            .ConfigureAwait(false))
        {
            throw new NotATransientException("Error during order request validation!");
        }

        var billingValidationEvent = new BillingValidationRequestEvent
        {
            UserId = context.Message.UserId,
            OrderId = context.Message.OrderId
        };

        await billingValidationEngineProducer
            .Produce(
                Guid.NewGuid().ToString(),
                billingValidationEvent,
                context.CancellationToken)
            .ConfigureAwait(false);

        await next.Execute(context)
            .ConfigureAwait(false);
    }

    async Task IStateMachineActivity<OrderRequestSagaInstance, OrderRequestEvent>.Faulted<TException>(
        BehaviorExceptionContext<OrderRequestSagaInstance, OrderRequestEvent, TException> context, 
        IBehavior<OrderRequestSagaInstance, OrderRequestEvent> next) 
        => await next.Faulted(context).ConfigureAwait(false);
    
    void IProbeSite.Probe(ProbeContext context) => context.CreateScope(nameof(ReceiveOrderRequestActivity));
    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    
}