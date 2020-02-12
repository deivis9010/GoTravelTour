using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandoprecioserrestriccion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "RestriccionesPrecios",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Habitaciones",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "RestriccionesPrecios");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Habitaciones");
        }
    }
}
