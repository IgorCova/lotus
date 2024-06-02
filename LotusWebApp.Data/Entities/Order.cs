using System.ComponentModel.DataAnnotations;

namespace LotusWebApp.Data.Entities;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public int SubsriptionId { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public string Status { get; set; }
}