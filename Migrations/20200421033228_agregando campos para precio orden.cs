using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandocamposparaprecioorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioOrden",
                table: "OrdenVehiculo",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioOrden",
                table: "OrdenTraslado",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioOrden",
                table: "OrdenAlojamiento",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioOrden",
                table: "OrdenActividad",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioGeneralOrden",
                table: "Orden",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioOrden",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "PrecioOrden",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "PrecioOrden",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "PrecioOrden",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "PrecioGeneralOrden",
                table: "Orden");
        }
    }
}
