using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace LotusWebApp.Data.Entities;

public class OrderState: SagaStateMachineInstance
{
    [Key]
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    [MaxLength(50)]
    public string? CurrentState { get; set; }
    public Guid OrderId { get; set; }
}