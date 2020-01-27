using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class refreshh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NombreTemporadas_Contratos_ContratoId",
                table: "NombreTemporadas");

            migrationBuilder.DropIndex(
                name: "IX_NombreTemporadas_ContratoId",
                table: "NombreTemporadas");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "NombreTemporadas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "NombreTemporadas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NombreTemporadas_ContratoId",
                table: "NombreTemporadas",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_NombreTemporadas_Contratos_ContratoId",
                table: "NombreTemporadas",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
