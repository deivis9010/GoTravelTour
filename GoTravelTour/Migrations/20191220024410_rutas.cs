using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class rutas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoDestinoPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoOrigenPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_puntoDestinoPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_puntoOrigenPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "puntoDestinoPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "puntoOrigenPuntoInteresId",
                table: "Rutas");

            migrationBuilder.AddColumn<int>(
                name: "puntoDestinoId",
                table: "Rutas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "puntoOrigenId",
                table: "Rutas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "puntoDestinoId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "puntoOrigenId",
                table: "Rutas");

            migrationBuilder.AddColumn<int>(
                name: "puntoDestinoPuntoInteresId",
                table: "Rutas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "puntoOrigenPuntoInteresId",
                table: "Rutas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoDestinoPuntoInteresId",
                table: "Rutas",
                column: "puntoDestinoPuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_puntoOrigenPuntoInteresId",
                table: "Rutas",
                column: "puntoOrigenPuntoInteresId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoDestinoPuntoInteresId",
                table: "Rutas",
                column: "puntoDestinoPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoOrigenPuntoInteresId",
                table: "Rutas",
                column: "puntoOrigenPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
