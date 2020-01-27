using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class rutas4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoDestinoPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_puntoOrigenPuntoInteresId",
                table: "Rutas");

            migrationBuilder.RenameColumn(
                name: "puntoOrigenPuntoInteresId",
                table: "Rutas",
                newName: "PuntoInteresOrigenPuntoInteresId");

            migrationBuilder.RenameColumn(
                name: "puntoDestinoPuntoInteresId",
                table: "Rutas",
                newName: "PuntoInteresDestinoPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutas_puntoOrigenPuntoInteresId",
                table: "Rutas",
                newName: "IX_Rutas_PuntoInteresOrigenPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutas_puntoDestinoPuntoInteresId",
                table: "Rutas",
                newName: "IX_Rutas_PuntoInteresDestinoPuntoInteresId");

            migrationBuilder.AddColumn<int>(
                name: "Distancia",
                table: "Rutas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_PuntoInteresDestinoPuntoInteresId",
                table: "Rutas",
                column: "PuntoInteresDestinoPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_PuntosInteres_PuntoInteresOrigenPuntoInteresId",
                table: "Rutas",
                column: "PuntoInteresOrigenPuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_PuntoInteresDestinoPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_PuntosInteres_PuntoInteresOrigenPuntoInteresId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "Distancia",
                table: "Rutas");

            migrationBuilder.RenameColumn(
                name: "PuntoInteresOrigenPuntoInteresId",
                table: "Rutas",
                newName: "puntoOrigenPuntoInteresId");

            migrationBuilder.RenameColumn(
                name: "PuntoInteresDestinoPuntoInteresId",
                table: "Rutas",
                newName: "puntoDestinoPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutas_PuntoInteresOrigenPuntoInteresId",
                table: "Rutas",
                newName: "IX_Rutas_puntoOrigenPuntoInteresId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutas_PuntoInteresDestinoPuntoInteresId",
                table: "Rutas",
                newName: "IX_Rutas_puntoDestinoPuntoInteresId");

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
