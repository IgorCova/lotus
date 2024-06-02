using LotusWebApp.Data.Models.Saga;
using LotusWebApp.Saga.StateMachines.CustomActivities;
using MassTransit;

namespace LotusWebApp.Saga.StateMachines;

public sealed class OrderRequestStateMachine : MassTransitStateMachine<OrderRequestSagaInstance>
{
    public OrderRequestStateMachine()
    {
        // Registering known states
        RegisterStates();
        // Registering and correlating events
        CorrelateEvents();
        
        Initially(
            When(OrderRequestedEvent)
                .InitializeSaga()
                .Activity(config => config.OfType<ReceiveOrderRequestActivity>())
                .TransitionTo(ValidatingBilling),
            When(FaultEvent)
                .Activity(config => config.OfType<ProcessFaultActivity>())
                .TransitionTo(Faulted));

        During(ValidatingBilling,
            When(BillingValidationResponseEvent)
                .UpdateSaga()
                .Activity(config => config.OfType<BillingValidationActivity>())
                .TransitionTo(ValidatingStock),
            When(FaultEvent)
                .Activity(config => config.OfType<ProcessFaultActivity>())
                .TransitionTo(Faulted));

        During(ValidatingStock,
            When(StockValidationResponseEvent)
                .UpdateSaga()
                .Activity(config => config.OfType<StockValidationActivity>())
                .TransitionTo(ValidatingDelivery),
            When(FaultEvent)
                .Activity(config => config.OfType<ProcessFaultActivity>())
                .TransitionTo(Faulted));

        During(ValidatingDelivery,
            When(DeliveryValidationResponseEvent)
                .UpdateSaga()
                .Activity(config => config.OfType<DeliveryValidationActivity>())
                .TransitionTo(NotifyingSourceSystem),
            When(FaultEvent)
                .Activity(config => config.OfType<ProcessFaultActivity>())
                .TransitionTo(Faulted));
        
        WhenEnter(Faulted,
            context => context.TransitionTo(NotifyingSourceSystem));

        WhenEnter(NotifyingSourceSystem,
            activityCallback => activityCallback
                .NotifySourceSystem()
                .Finalize());

        During(Final,
            Ignore(OrderRequestedEvent),
            Ignore(BillingValidationResponseEvent),
            Ignore(StockValidationResponseEvent),
            Ignore(DeliveryValidationResponseEvent),
            Ignore(FaultEvent));
        
        // Delete finished saga instances from the repository
        SetCompletedWhenFinalized();
    }

    private void CorrelateEvents()
    {
        Event(() => OrderRequestedEvent, x => x
            .CorrelateById(m => m.Message.OrderId));
        
        Event(() => BillingValidationResponseEvent, x => x
            .CorrelateById(m => m.Message.OrderId));
        
        Event(() => StockValidationResponseEvent, x => x
            .CorrelateById(m => m.Message.OrderId));

        Event(() => DeliveryValidationResponseEvent, x => x
            .CorrelateById(m => m.Message.OrderId));
        
        Event(() => FaultEvent, x => x
            .CorrelateById(m => m.CorrelationId ?? new Guid())
            .SelectId(m => m.CorrelationId ?? new Guid())
            .OnMissingInstance(m => m.Discard()));
    }

    private void RegisterStates() 
        => InstanceState(x => x.CurrentState);
    
    public State? ValidatingBilling { get; set; }
    public State? ValidatingStock { get; set; }
    public State? ValidatingDelivery { get; set; }
    public State? NotifyingSourceSystem { get; set; }
    public State? Faulted { get; set; }
    public Event<OrderRequestEvent>? OrderRequestedEvent { get; set; }
    public Event<BillingValidationResponseEvent>? BillingValidationResponseEvent { get; set; }
    public Event<StockValidationResponseEvent>? StockValidationResponseEvent { get; set; }
    public Event<DeliveryValidationResponseEvent>? DeliveryValidationResponseEvent { get; set; }
    public Event<FaultMessageEvent>? FaultEvent { get; set; }
}