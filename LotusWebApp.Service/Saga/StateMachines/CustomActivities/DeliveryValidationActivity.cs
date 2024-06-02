using LotusWebApp.Data.Models.Exceptions;
using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Saga.Services;
using MassTransit;

namespace LotusWebApp.Saga.StateMachines.CustomActivities;

public sealed class DeliveryValidationActivity(IApiService apiService)
    : IStateMachineActivity<OrderRequestSagaInstance, DeliveryValidationResponseEvent>
{
    async Task IStateMachineActivity<OrderRequestSagaInstance, DeliveryValidationResponseEvent>.Execute(
        BehaviorContext<OrderRequestSagaInstance, DeliveryValidationResponseEvent> context,
        IBehavior<OrderRequestSagaInstance, DeliveryValidationResponseEvent> next)
    {
        if (!await apiService.ValidateIncomingDeliveryResult(context.Message))
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
                DeliveryValidation = context.Message
            }
        };

        await next
            .Execute(context)
            .ConfigureAwait(false);
    }

    async Task IStateMachineActivity<OrderRequestSagaInstance, DeliveryValidationResponseEvent>.Faulted<TException>(
        BehaviorExceptionContext<OrderRequestSagaInstance, DeliveryValidationResponseEvent, TException> context,
        IBehavior<OrderRequestSagaInstance, DeliveryValidationResponseEvent> next)
            => await next.Faulted(context).ConfigureAwait(false);

    void IProbeSite.Probe(ProbeContext context) => context.CreateScope(nameof(ReceiveOrderRequestActivity));
    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}