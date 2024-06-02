using LotusWebApp.Data.Models.Saga;

namespace LotusWebApp.Saga.Services
{
    public sealed class ApiService : IApiService
    {
        public Task<bool> ValidateIncomingOrderRequestAsync(OrderRequestEvent @event)
             => Task.FromResult(@event.UserId.Equals("ERROR", StringComparison.CurrentCultureIgnoreCase));

        public Task<bool> ValidateIncomingBillingResult(BillingValidationResponseEvent @event)
            => Task.FromResult(true/*@event.HasMoney*/);

        public Task<bool> ValidateIncomingStockResult(StockValidationResponseEvent @event)
            => Task.FromResult(true/*@event.StockAvailable*/);

        public Task<bool> ValidateIncomingDeliveryResult(DeliveryValidationResponseEvent @event)
            => Task.FromResult(true/*@event.DeliveryAvailable*/);
    }
}