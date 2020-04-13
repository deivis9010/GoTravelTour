using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class contratoenprecioplanes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dososContratoId",
                table: "PrecioPlanesAlimenticios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_dososContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "dososContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_dososContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "dososContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_dososContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_PrecioPlanesAlimenticios_dososContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropColumn(
                name: "dososContratoId",
                table: "PrecioPlanesAlimenticios");
        }
    }
}
