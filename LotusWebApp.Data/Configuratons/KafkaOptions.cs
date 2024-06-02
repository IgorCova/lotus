using Confluent.Kafka;

namespace LotusWebApp.Data.Models.Saga;

public sealed record KafkaOptions
{
    public string ConsumerGroup { get; set; } = default!;
    public Topics Topics { get; set; } = default!;
    public ClientConfig ClientConfig { get; set; } = default!;
};

public sealed record Topics
{
    public string OrderManagementSystemRequest { get; set; } = default!;
    public string OrderManagementSystemResponse { get; set; } = default!;
    public string StockValidationEngineRequest { get; set; } = default!;
    public string StockValidationEngineResponse { get; set; } = default!;
    public string BillingValidationEngineRequest { get; set; } = default!;
    public string BillingValidationEngineResponse { get; set; } = default!;
    public string DeliveryValidationEngineRequest { get; set; } = default!;
    public string DeliveryValidationEngineResponse { get; set; } = default!;
    public string Error { get; set; } = default!;
}