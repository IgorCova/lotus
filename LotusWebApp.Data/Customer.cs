using System.ComponentModel.DataAnnotations;

namespace LotusWebApp.Data;

public class Customer
{
    [Key]
    public Guid Id {get; set; }
    public DateTime CreatedDate {get; set; }
    public string Name {get; set; }
    public string Role {get; set; }
}