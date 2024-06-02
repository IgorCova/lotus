using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LotusWebApp.Data.SagaMigrations
{
    /// <inheritdoc />
    public partial class NullBaleEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_BillingValidationResponseEvent_BillingVa~",
                table: "OrderResponseEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_DeliveryValidationResponseEvent_Delivery~",
                table: "OrderResponseEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_StockValidationResponseEvent_StockValida~",
                table: "OrderResponseEvent");

            migrationBuilder.AlterColumn<int>(
                name: "StockValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "BillingValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_BillingValidationResponseEvent_BillingVa~",
                table: "OrderResponseEvent",
                column: "BillingValidationId",
                principalTable: "BillingValidationResponseEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_DeliveryValidationResponseEvent_Delivery~",
                table: "OrderResponseEvent",
                column: "DeliveryValidationId",
                principalTable: "DeliveryValidationResponseEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_StockValidationResponseEvent_StockValida~",
                table: "OrderResponseEvent",
                column: "StockValidationId",
                principalTable: "StockValidationResponseEvent",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_BillingValidationResponseEvent_BillingVa~",
                table: "OrderResponseEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_DeliveryValidationResponseEvent_Delivery~",
                table: "OrderResponseEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderResponseEvent_StockValidationResponseEvent_StockValida~",
                table: "OrderResponseEvent");

            migrationBuilder.AlterColumn<int>(
                name: "StockValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BillingValidationId",
                table: "OrderResponseEvent",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_BillingValidationResponseEvent_BillingVa~",
                table: "OrderResponseEvent",
                column: "BillingValidationId",
                principalTable: "BillingValidationResponseEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_DeliveryValidationResponseEvent_Delivery~",
                table: "OrderResponseEvent",
                column: "DeliveryValidationId",
                principalTable: "DeliveryValidationResponseEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderResponseEvent_StockValidationResponseEvent_StockValida~",
                table: "OrderResponseEvent",
                column: "StockValidationId",
                principalTable: "StockValidationResponseEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
