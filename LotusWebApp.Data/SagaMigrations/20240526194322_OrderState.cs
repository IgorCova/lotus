using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LotusWebApp.Data.SagaMigrations
{
    /// <inheritdoc />
    public partial class OrderState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingValidationResponseEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HasMoney = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingValidationResponseEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryValidationResponseEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeliveryAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryValidationResponseEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockValidationResponseEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionId = table.Column<string>(type: "text", nullable: false),
                    StockAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockValidationResponseEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderResponseEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CustomerType = table.Column<string>(type: "text", nullable: false),
                    BillingValidationId = table.Column<int>(type: "integer", nullable: false),
                    StockValidationId = table.Column<int>(type: "integer", nullable: false),
                    DeliveryValidationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderResponseEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderResponseEvent_BillingValidationResponseEvent_BillingVa~",
                        column: x => x.BillingValidationId,
                        principalTable: "BillingValidationResponseEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderResponseEvent_DeliveryValidationResponseEvent_Delivery~",
                        column: x => x.DeliveryValidationId,
                        principalTable: "DeliveryValidationResponseEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderResponseEvent_StockValidationResponseEvent_StockValida~",
                        column: x => x.StockValidationId,
                        principalTable: "StockValidationResponseEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationReply<OrderResponseEvent>",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    DataId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationReply<OrderResponseEvent>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationReply<OrderResponseEvent>_OrderResponseEvent_Da~",
                        column: x => x.DataId,
                        principalTable: "OrderResponseEvent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentState = table.Column<string>(type: "text", nullable: true),
                    SubscriptionId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CustomerType = table.Column<string>(type: "text", nullable: true),
                    HasMoney = table.Column<bool>(type: "boolean", nullable: true),
                    NotificationReplyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderState_NotificationReply<OrderResponseEvent>_Notificati~",
                        column: x => x.NotificationReplyId,
                        principalTable: "NotificationReply<OrderResponseEvent>",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReply<OrderResponseEvent>_DataId",
                table: "NotificationReply<OrderResponseEvent>",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderResponseEvent_BillingValidationId",
                table: "OrderResponseEvent",
                column: "BillingValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderResponseEvent_DeliveryValidationId",
                table: "OrderResponseEvent",
                column: "DeliveryValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderResponseEvent_StockValidationId",
                table: "OrderResponseEvent",
                column: "StockValidationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderState_NotificationReplyId",
                table: "OrderState",
                column: "NotificationReplyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderState");

            migrationBuilder.DropTable(
                name: "NotificationReply<OrderResponseEvent>");

            migrationBuilder.DropTable(
                name: "OrderResponseEvent");

            migrationBuilder.DropTable(
                name: "BillingValidationResponseEvent");

            migrationBuilder.DropTable(
                name: "DeliveryValidationResponseEvent");

            migrationBuilder.DropTable(
                name: "StockValidationResponseEvent");
        }
    }
}
