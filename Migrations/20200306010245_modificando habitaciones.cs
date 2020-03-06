using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandohabitaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaHabitacionId",
                table: "Habitaciones",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NombreHabitacionId",
                table: "Habitaciones",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_CategoriaHabitacionId",
                table: "Habitaciones",
                column: "CategoriaHabitacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Habitaciones_NombreHabitacionId",
                table: "Habitaciones",
                column: "NombreHabitacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habitaciones_CategoriaHabitacion_CategoriaHabitacionId",
                table: "Habitaciones",
                column: "CategoriaHabitacionId",
                principalTable: "CategoriaHabitacion",
                principalColumn: "CategoriaHabitacionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Habitaciones_NombreHabitacion_NombreHabitacionId",
                table: "Habitaciones",
                column: "NombreHabitacionId",
                principalTable: "NombreHabitacion",
                principalColumn: "NombreHabitacionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habitaciones_CategoriaHabitacion_CategoriaHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Habitaciones_NombreHabitacion_NombreHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Habitaciones_CategoriaHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropIndex(
                name: "IX_Habitaciones_NombreHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "CategoriaHabitacionId",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "NombreHabitacionId",
                table: "Habitaciones");
        }
    }
}
