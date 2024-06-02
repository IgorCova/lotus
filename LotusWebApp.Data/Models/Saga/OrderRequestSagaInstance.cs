using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace LotusWebApp.Data.Models.Saga;

public class OrderRequestSagaInstance : SagaStateMachineInstance, ISagaVersion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string? CurrentState { get; set; }
    public string? SubscriptionId { get; set; }
    public string? UserId { get; set; }
    public string? CustomerType { get; set; } = "User";
    public bool? HasMoney { get; set; }
    public Guid OrderId { get; set; }
    public NotificationReply<OrderResponseEvent>? NotificationReply { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Default props
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
}