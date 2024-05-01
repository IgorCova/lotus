using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LotusWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdduseridToPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Userid",
                table: "Pages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Userid",
                table: "Pages");
        }
    }
}
