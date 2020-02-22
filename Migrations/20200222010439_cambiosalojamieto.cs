using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiosalojamieto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckIn",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckOut",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadAdultoMax",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadAdultoMin",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadInfanteMax",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadInfanteMin",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadNinoMax",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadNinoMin",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoLegal",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteMascota",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliticaCancelacion",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliticaNino",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuntoInteresId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CheckOut",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadAdultoMax",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadAdultoMin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadInfanteMax",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadInfanteMin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadNinoMax",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "EdadNinoMin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "InfoLegal",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PermiteMascota",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PoliticaCancelacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PoliticaNino",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PuntoInteresId",
                table: "Productos");
        }
    }
}
