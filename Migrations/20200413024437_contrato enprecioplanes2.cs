using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class contratoenprecioplanes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_dososContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.RenameColumn(
                name: "dososContratoId",
                table: "PrecioPlanesAlimenticios",
                newName: "ContratoDelPrecioContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_PrecioPlanesAlimenticios_dososContratoId",
                table: "PrecioPlanesAlimenticios",
                newName: "IX_PrecioPlanesAlimenticios_ContratoDelPrecioContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_ContratoDelPrecioContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "ContratoDelPrecioContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_ContratoDelPrecioContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.RenameColumn(
                name: "ContratoDelPrecioContratoId",
                table: "PrecioPlanesAlimenticios",
                newName: "dososContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_PrecioPlanesAlimenticios_ContratoDelPrecioContratoId",
                table: "PrecioPlanesAlimenticios",
                newName: "IX_PrecioPlanesAlimenticios_dososContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_dososContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "dososContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
