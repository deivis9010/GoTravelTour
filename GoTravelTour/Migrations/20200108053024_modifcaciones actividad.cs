using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifcacionesactividad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoAsistencia",
                table: "Productos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TelefonoAsistencia",
                table: "Productos");
        }
    }
}
