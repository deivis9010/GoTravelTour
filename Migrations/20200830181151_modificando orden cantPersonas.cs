using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoordencantPersonas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadAdulto",
                table: "Orden",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadInfante",
                table: "Orden",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadNino",
                table: "Orden",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadAdulto",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "CantidadInfante",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "CantidadNino",
                table: "Orden");
        }
    }
}
