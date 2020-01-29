using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class PrecioServicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicio_Temporadas_TemporadaId",
                table: "Servicio");

            migrationBuilder.DropIndex(
                name: "IX_Servicio_TemporadaId",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "HorasAdicionales",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "Incluido",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "PrecioAdulto",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "PrecioInfante",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "PrecioNino",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "TemporadaId",
                table: "Servicio");

            migrationBuilder.CreateTable(
                name: "PrecioServicio",
                columns: table => new
                {
                    PrecioServicioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HorasAdicionales = table.Column<int>(nullable: false),
                    Incluido = table.Column<decimal>(nullable: true),
                    PrecioAdulto = table.Column<decimal>(nullable: true),
                    PrecioNino = table.Column<decimal>(nullable: true),
                    PrecioInfante = table.Column<decimal>(nullable: true),
                    ProductoId = table.Column<int>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioServicio", x => x.PrecioServicioId);
                    table.ForeignKey(
                        name: "FK_PrecioServicio_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioServicio_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrecioServicio_ProductoId",
                table: "PrecioServicio",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioServicio_TemporadaId",
                table: "PrecioServicio",
                column: "TemporadaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrecioServicio");

            migrationBuilder.AddColumn<int>(
                name: "HorasAdicionales",
                table: "Servicio",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Incluido",
                table: "Servicio",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioAdulto",
                table: "Servicio",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioInfante",
                table: "Servicio",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioNino",
                table: "Servicio",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemporadaId",
                table: "Servicio",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_TemporadaId",
                table: "Servicio",
                column: "TemporadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicio_Temporadas_TemporadaId",
                table: "Servicio",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "TemporadaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
