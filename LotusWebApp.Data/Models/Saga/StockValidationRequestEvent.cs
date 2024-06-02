namespace LotusWebApp.Data.Models.Saga;

public record StockValidationRequestEvent
{
    public string SubscriptionId { get; set; } = default!;
    public Guid OrderId { get; set; } = default!;
}