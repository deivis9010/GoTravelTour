using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class billfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdBillQB",
                table: "OrdenVehiculo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdBillQB",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdBillQB",
                table: "OrdenAlojamiento",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdBillQB",
                table: "OrdenActividad",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdBillQB",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "IdBillQB",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "IdBillQB",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "IdBillQB",
                table: "OrdenActividad");
        }
    }
}
