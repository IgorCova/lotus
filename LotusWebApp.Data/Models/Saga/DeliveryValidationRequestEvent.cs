namespace LotusWebApp.Data.Models.Saga;

public record DeliveryValidationRequestEvent
{
    public string UserId { get; set; } = string.Empty;
    public int SubscriptionId { get; set; }
    public Guid OrderId { get; set; }

}