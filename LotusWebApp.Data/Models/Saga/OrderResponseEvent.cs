using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotusWebApp.Data.Models.Saga;

public record OrderResponseEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string CustomerType { get; set; } = "User";
    public BillingValidationResponseEvent? BillingValidation { get; set; } = default!;
    public StockValidationResponseEvent? StockValidation { get; set; } = default!;
    public DeliveryValidationResponseEvent? DeliveryValidation { get; set; } = default!;
}