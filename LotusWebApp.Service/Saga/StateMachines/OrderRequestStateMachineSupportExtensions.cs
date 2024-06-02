using LotusWebApp.Data.Models.Saga;
using MassTransit;
using MassTransit.SagaStateMachine;

namespace LotusWebApp.Saga.StateMachines;

public static class OrderRequestStateMachineSupportExtensions
{
    public static EventActivityBinder<OrderRequestSagaInstance, OrderRequestEvent> InitializeSaga(
        this EventActivityBinder<OrderRequestSagaInstance, OrderRequestEvent> binder)
        => binder.Then(context =>
            {
                LogContext.Info?.Log("Order requested: {0}", context.CorrelationId);

                context.Saga.CreatedAt = DateTime.Now;
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.SubscriptionId = context.Message.SubscriptionId;
                context.Saga.UserId = context.Message.UserId;
            });
    
    public static EventActivityBinder<OrderRequestSagaInstance, BillingValidationResponseEvent> UpdateSaga(
        this EventActivityBinder<OrderRequestSagaInstance, BillingValidationResponseEvent> binder)
        => binder.Then(context =>
            {
                LogContext.Info?.Log("Billing validation: {0}", context.CorrelationId);
                
                context.Saga.HasMoney = context.Message.HasMoney;
                context.Saga.UpdatedAt = DateTime.Now;
            });
    
    public static EventActivityBinder<OrderRequestSagaInstance, StockValidationResponseEvent> UpdateSaga(
        this EventActivityBinder<OrderRequestSagaInstance, StockValidationResponseEvent> binder)
        => binder.Then(context =>
            {
                LogContext.Info?.Log("Stock validation: {0}", context.CorrelationId);
                
                context.Saga.UpdatedAt = DateTime.Now;
            });

    public static EventActivityBinder<OrderRequestSagaInstance, DeliveryValidationResponseEvent> UpdateSaga(
        this EventActivityBinder<OrderRequestSagaInstance, DeliveryValidationResponseEvent> binder)
        => binder.Then(context =>
        {
            LogContext.Info?.Log("Delivery validation: {0}", context.CorrelationId);

            context.Saga.UpdatedAt = DateTime.Now;
        });
    
    public static EventActivityBinder<OrderRequestSagaInstance> NotifySourceSystem(
        this EventActivityBinder<OrderRequestSagaInstance> binder)
    {
        Func<BehaviorContext<OrderRequestSagaInstance>, Task> asyncAction = async context =>
        {
            LogContext.Info?.Log("Notifying source system: {0}", context.CorrelationId);
            
            await context.GetServiceOrCreateInstance<ITopicProducer<string, NotificationReply<OrderResponseEvent>>>()
                .Produce(
                    Guid.NewGuid().ToString(),
                    context.Saga.NotificationReply!,
                    context.CancellationToken)
                .ConfigureAwait(false);
        };
        
        return binder.Add(new AsyncActivity<OrderRequestSagaInstance>(asyncAction));
    }
}