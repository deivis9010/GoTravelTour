using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class temporadasamodificadores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModificadorTemporada",
                columns: table => new
                {
                    ModificadorTemporadaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModificadorId = table.Column<int>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificadorTemporada", x => x.ModificadorTemporadaId);
                    table.ForeignKey(
                        name: "FK_ModificadorTemporada_Modificadores_ModificadorId",
                        column: x => x.ModificadorId,
                        principalTable: "Modificadores",
                        principalColumn: "ModificadorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModificadorTemporada_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorTemporada_ModificadorId",
                table: "ModificadorTemporada",
                column: "ModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorTemporada_TemporadaId",
                table: "ModificadorTemporada",
                column: "TemporadaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModificadorTemporada");
        }
    }
}
