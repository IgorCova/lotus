using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotusWebApp.Data.Models.Saga;

public sealed class NotificationReply<TMessage>
    where TMessage: class
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool Success { get; set; }
    public string? Reason { get; set; }
    public TMessage? Data { get; set; }
}