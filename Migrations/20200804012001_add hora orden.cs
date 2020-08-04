using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addhoraorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoraFin",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraInicio",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraFin",
                table: "OrdenTraslado",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraInicio",
                table: "OrdenTraslado",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraFin",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraInicio",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraFin",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoraInicio",
                table: "OrdenActividad",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "OrdenActividad");
        }
    }
}
