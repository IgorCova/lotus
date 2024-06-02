using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotusWebApp.Data.Models.Saga;

public record DeliveryValidationResponseEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool DeliveryAvailable { get; set; } = default!;
    public Guid OrderId { get; set; } = default!;


}