using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class add_TrasladoPrecios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacidadTraslado",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeloTraslado",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrasladoId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrecioTraslados",
                columns: table => new
                {
                    PrecioTrasladoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    RutasId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: true),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioTraslados", x => x.PrecioTrasladoId);
                    table.ForeignKey(
                        name: "FK_PrecioTraslados_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioTraslados_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioTraslados_Rutas_RutasId",
                        column: x => x.RutasId,
                        principalTable: "Rutas",
                        principalColumn: "RutasId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioTraslados_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrecioTraslados_ContratoId",
                table: "PrecioTraslados",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioTraslados_ProductoId",
                table: "PrecioTraslados",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioTraslados_RutasId",
                table: "PrecioTraslados",
                column: "RutasId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioTraslados_TemporadaId",
                table: "PrecioTraslados",
                column: "TemporadaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrecioTraslados");

            migrationBuilder.DropColumn(
                name: "CapacidadTraslado",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ModeloTraslado",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TrasladoId",
                table: "Productos");
        }
    }
}
