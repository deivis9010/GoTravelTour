using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "PrecioActividad",
                newName: "PrecioNino");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioAdulto",
                table: "PrecioActividad",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioInfante",
                table: "PrecioActividad",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioAdulto",
                table: "PrecioActividad");

            migrationBuilder.DropColumn(
                name: "PrecioInfante",
                table: "PrecioActividad");

            migrationBuilder.RenameColumn(
                name: "PrecioNino",
                table: "PrecioActividad",
                newName: "Precio");
        }
    }
}
