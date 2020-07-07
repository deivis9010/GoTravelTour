using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addModOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAlojamiento_ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento",
                column: "ModificadorAplicadoModificadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_Modificadores_ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento",
                column: "ModificadorAplicadoModificadorId",
                principalTable: "Modificadores",
                principalColumn: "ModificadorId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_Modificadores_ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_OrdenAlojamiento_ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropColumn(
                name: "ModificadorAplicadoModificadorId",
                table: "OrdenAlojamiento");
        }
    }
}
