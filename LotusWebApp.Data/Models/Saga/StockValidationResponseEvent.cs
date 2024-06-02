using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotusWebApp.Data.Models.Saga;

public record StockValidationResponseEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string SubscriptionId { get; set; } = default!;
    public bool StockAvailable { get; set; } = default!;
    public Guid OrderId { get; set; } = default!;

}