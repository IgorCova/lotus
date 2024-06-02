namespace LotusWebApp.Data.Models.Saga;

public record FaultMessageEvent
{
    public object? Event { get; set; } = default!;
    public string? ExceptionMessage { get; set; }
}