using LotusWebApp.Data;
using LotusWebApp.Data.Models.Saga;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Confluent.Kafka;
using LotusWebApp.Consumers;
using LotusWebApp.Saga.StateMachines;
using LotusWebApp.Saga.Transports;
using MassTransit.Transports;

namespace LotusWebApp.Saga;

public static class KafkaRegistrationExtensions
{
    public static IServiceCollection AddCustomKafka(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var kafkaOptions = configuration
            .GetSection(nameof(KafkaOptions))
            .Get<KafkaOptions>();

        ArgumentNullException.ThrowIfNull(kafkaOptions, nameof(kafkaOptions));

        services.AddMassTransit(massTransit =>
        {
            massTransit.UsingInMemory((context, cfg) =>  cfg.ConfigureEndpoints(context));
            massTransit.AddRider(rider =>
            {
                rider.AddTransient<IErrorTransport, FaultTransport>();

                rider
                    .AddSagaStateMachine<OrderRequestStateMachine, OrderRequestSagaInstance>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        r.ExistingDbContext<ApplicationDbContext>();
                        r.LockStatementProvider = new PostgresLockStatementProvider();
                    })
                    .Endpoint(e =>
                        {
                            e.Name = "Sravni.Deposit.Saga.Catalog";
                        }
                    );

                rider.AddProducer<string, NotificationReply<OrderResponseEvent>>(kafkaOptions.Topics.OrderManagementSystemResponse);
                rider.AddProducer<string, StockValidationRequestEvent>(kafkaOptions.Topics.StockValidationEngineRequest);
                rider.AddProducer<string, BillingValidationRequestEvent>(kafkaOptions.Topics.BillingValidationEngineRequest);
                rider.AddProducer<string, DeliveryValidationRequestEvent>(kafkaOptions.Topics.DeliveryValidationEngineRequest);
                rider.AddProducer<string, FaultMessageEvent>(kafkaOptions.Topics.Error);
                rider.AddProducer<string, OrderRequestEvent>(kafkaOptions.Topics.OrderManagementSystemRequest);

                rider.AddProducer<string, StockValidationResponseEvent>(kafkaOptions.Topics.StockValidationEngineResponse);
                rider.AddConsumersFromNamespaceContaining<StockValidationConsumer>();

                rider.AddProducer<string, BillingValidationResponseEvent>(kafkaOptions.Topics.BillingValidationEngineResponse);
                rider.AddConsumersFromNamespaceContaining<BillingValidationConsumer>();

                rider.AddProducer<string, DeliveryValidationResponseEvent>(kafkaOptions.Topics.DeliveryValidationEngineResponse);
                rider.AddConsumersFromNamespaceContaining<DeliveryValidationConsumer>();

                rider.UsingKafka(kafkaOptions.ClientConfig, (riderContext, kafkaConfig) =>
                {
                    kafkaConfig.TopicEndpoint<string, OrderRequestEvent>(
                       topicName: kafkaOptions.Topics.OrderManagementSystemRequest,
                       groupId: kafkaOptions.ConsumerGroup,
                       configure: topicConfig =>
                       {
                           topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                           topicConfig.DiscardSkippedMessages();
                           topicConfig.ConfigureSaga<OrderRequestSagaInstance>(riderContext);
                           topicConfig.UseInMemoryOutbox(riderContext);
                       });

                    kafkaConfig.TopicEndpoint<string, StockValidationResponseEvent>(
                       topicName: kafkaOptions.Topics.StockValidationEngineResponse,
                       groupId: kafkaOptions.ConsumerGroup,
                       configure: topicConfig =>
                       {
                           topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                           topicConfig.DiscardSkippedMessages();
                           topicConfig.ConfigureSaga<OrderRequestSagaInstance>(riderContext);
                           topicConfig.UseInMemoryOutbox(riderContext);
                       });

                    kafkaConfig.TopicEndpoint<string, BillingValidationResponseEvent>(
                       topicName: kafkaOptions.Topics.BillingValidationEngineResponse,
                       groupId: kafkaOptions.ConsumerGroup,
                       configure: topicConfig =>
                       {
                           topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                           topicConfig.DiscardSkippedMessages();
                           topicConfig.ConfigureSaga<OrderRequestSagaInstance>(riderContext);
                           topicConfig.UseInMemoryOutbox(riderContext);
                       });

                    kafkaConfig.TopicEndpoint<string, DeliveryValidationResponseEvent>(
                        topicName: kafkaOptions.Topics.DeliveryValidationEngineResponse,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.DiscardSkippedMessages();
                            topicConfig.ConfigureSaga<OrderRequestSagaInstance>(riderContext);
                            topicConfig.UseInMemoryOutbox(riderContext);
                        });

                    kafkaConfig.TopicEndpoint<string, FaultMessageEvent>(
                      topicName: kafkaOptions.Topics.Error,
                      groupId: kafkaOptions.Topics.Error,
                      configure: topicConfig =>
                      {
                          topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                          topicConfig.DiscardSkippedMessages();
                          topicConfig.ConfigureSaga<OrderRequestSagaInstance>(riderContext);
                          topicConfig.UseInMemoryOutbox(riderContext);

                      });

                    kafkaConfig.TopicEndpoint<string, StockValidationRequestEvent>(
                        topicName: kafkaOptions.Topics.StockValidationEngineRequest,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureConsumer<StockValidationConsumer>(riderContext);
                            topicConfig.DiscardSkippedMessages();
                        });

                    kafkaConfig.TopicEndpoint<string, BillingValidationRequestEvent>(
                        topicName: kafkaOptions.Topics.BillingValidationEngineRequest,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureConsumer<BillingValidationConsumer>(riderContext);
                            topicConfig.DiscardSkippedMessages();
                        });

                    kafkaConfig.TopicEndpoint<string, DeliveryValidationRequestEvent>(
                        topicName: kafkaOptions.Topics.DeliveryValidationEngineRequest,
                        groupId: kafkaOptions.ConsumerGroup,
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureConsumer<DeliveryValidationConsumer>(riderContext);
                            topicConfig.DiscardSkippedMessages();
                        });
                });
            });
        });

        return services;
    }
}