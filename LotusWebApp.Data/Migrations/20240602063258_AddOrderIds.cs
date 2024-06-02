using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LotusWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "StockValidationResponseEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "DeliveryValidationResponseEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "BillingValidationResponseEvent",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "StockValidationResponseEvent");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "DeliveryValidationResponseEvent");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "BillingValidationResponseEvent");
        }
    }
}
