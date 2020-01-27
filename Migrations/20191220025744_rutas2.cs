using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class rutas2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoDestinoId",
                table: "Rutas",
                column: "puntoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoOrigenId",
                table: "Rutas",
                column: "puntoOrigenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoDestinoId",
                table: "Rutas",
                column: "puntoDestinoId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoOrigenId",
                table: "Rutas",
                column: "puntoOrigenId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoDestinoId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoOrigenId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_puntoDestinoId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_puntoOrigenId",
                table: "Rutas");
        }
    }
}
