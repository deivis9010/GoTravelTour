using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class camposauxiliares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPersonas",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrecioAdultos",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrecioInfantes",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrecioNinos",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPersonas",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "TotalPrecioAdultos",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "TotalPrecioInfantes",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "TotalPrecioNinos",
                table: "OrdenServicioAdicional");
        }
    }
}
