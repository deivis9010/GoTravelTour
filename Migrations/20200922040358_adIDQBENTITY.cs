using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class adIDQBENTITY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdQB",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Alojamiento_IdQB",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Traslado_IdQB",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Vehiculo_IdQB",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdQB",
                table: "Habitaciones",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Traslado_IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Vehiculo_IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "IdQB",
                table: "Habitaciones");
        }
    }
}
