using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class re : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoHabitacionId",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_TipoHabitacionId",
                table: "OrdenAlojamiento",
                column: "TipoHabitacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_TipoHabitaciones_TipoHabitacionId",
                table: "OrdenAlojamiento",
                column: "TipoHabitacionId",
                principalTable: "TipoHabitaciones",
                principalColumn: "TipoHabitacionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_TipoHabitaciones_TipoHabitacionId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenAlojamiento_TipoHabitacionId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "TipoHabitacionId",
                table: "OrdenAlojamiento");
        }
    }
}
