using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_Contratos_ContratoId",
                table: "Modificadores");

            migrationBuilder.DropIndex(
                name: "IX_Modificadores_ContratoId",
                table: "Modificadores");

            migrationBuilder.AddColumn<int>(
                name: "TemporadaId",
                table: "Modificadores",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_TemporadaId",
                table: "Modificadores",
                column: "TemporadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_Temporadas_TemporadaId",
                table: "Modificadores",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "TemporadaId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_Temporadas_TemporadaId",
                table: "Modificadores");

            migrationBuilder.DropIndex(
                name: "IX_Modificadores_TemporadaId",
                table: "Modificadores");

            migrationBuilder.DropColumn(
                name: "TemporadaId",
                table: "Modificadores");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_ContratoId",
                table: "Modificadores",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_Contratos_ContratoId",
                table: "Modificadores",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
