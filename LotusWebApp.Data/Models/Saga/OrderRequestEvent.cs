namespace LotusWebApp.Data.Models.Saga;

public record OrderRequestEvent
{
    public Guid OrderId { get; set;} = default!;
    public string UserId { get; set;} = default!;
    public string SubscriptionId { get; set; } = default!;
}