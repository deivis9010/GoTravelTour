using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificaciondeprecioalojamiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Temporadas_TemporadaId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_PrecioPlanesAlimenticios_TemporadaId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropColumn(
                name: "TemporadaId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.AddColumn<int>(
                name: "PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoHabitacionId",
                table: "PrecioAlojamiento",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios",
                column: "PlanesAlimenticiosId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_TipoHabitacionId",
                table: "PrecioAlojamiento",
                column: "TipoHabitacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioAlojamiento_TipoHabitaciones_TipoHabitacionId",
                table: "PrecioAlojamiento",
                column: "TipoHabitacionId",
                principalTable: "TipoHabitaciones",
                principalColumn: "TipoHabitacionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_PlanesAlimenticios_PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios",
                column: "PlanesAlimenticiosId",
                principalTable: "PlanesAlimenticios",
                principalColumn: "PlanesAlimenticiosId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioAlojamiento_TipoHabitaciones_TipoHabitacionId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_PlanesAlimenticios_PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_PrecioPlanesAlimenticios_PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_PrecioAlojamiento_TipoHabitacionId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropColumn(
                name: "PlanesAlimenticiosId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropColumn(
                name: "TipoHabitacionId",
                table: "PrecioAlojamiento");

            migrationBuilder.AddColumn<int>(
                name: "TemporadaId",
                table: "PrecioPlanesAlimenticios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_TemporadaId",
                table: "PrecioPlanesAlimenticios",
                column: "TemporadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Temporadas_TemporadaId",
                table: "PrecioPlanesAlimenticios",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "TemporadaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
