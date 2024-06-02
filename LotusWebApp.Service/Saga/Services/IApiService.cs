using LotusWebApp.Data.Models.Saga;

namespace LotusWebApp.Saga.Services
{
    public interface IApiService
    {
        Task<bool> ValidateIncomingOrderRequestAsync(OrderRequestEvent @event);
        Task<bool> ValidateIncomingBillingResult(BillingValidationResponseEvent @event);
        Task<bool> ValidateIncomingStockResult(StockValidationResponseEvent @event);
        Task<bool> ValidateIncomingDeliveryResult(DeliveryValidationResponseEvent @event);
    }
}