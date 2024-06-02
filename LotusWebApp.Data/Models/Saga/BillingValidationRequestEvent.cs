namespace LotusWebApp.Data.Models.Saga;

public record BillingValidationRequestEvent
{
    public string UserId { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
}