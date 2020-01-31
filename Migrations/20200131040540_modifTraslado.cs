using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifTraslado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModeloTraslado",
                table: "Productos",
                newName: "TipoTraslado");

            migrationBuilder.AddColumn<int>(
                name: "CantidadAdultTras",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadInfantesTras",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadNinoTras",
                table: "Productos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadAdultTras",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CantidadInfantesTras",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CantidadNinoTras",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "TipoTraslado",
                table: "Productos",
                newName: "ModeloTraslado");
        }
    }
}
