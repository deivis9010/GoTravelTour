using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoubicacionIDqb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alojamiento_IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Traslado_IdQB",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Vehiculo_IdQB",
                table: "Productos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
