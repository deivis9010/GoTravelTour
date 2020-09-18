using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addOrdenQBUtils : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BillCreated",
                table: "Orden",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EstimatedCreated",
                table: "Orden",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InvoiceCreated",
                table: "Orden",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillCreated",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "EstimatedCreated",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "InvoiceCreated",
                table: "Orden");
        }
    }
}
