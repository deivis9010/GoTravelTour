using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class eliminandoprecioactividad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioAlojamiento_Contratos_ContratoId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_ContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropForeignKey(
                name: "FK_PrecioRentaAutos_Contratos_ContratoId",
                table: "PrecioRentaAutos");

            migrationBuilder.DropForeignKey(
                name: "FK_PrecioTraslados_Contratos_ContratoId",
                table: "PrecioTraslados");

            migrationBuilder.DropTable(
                name: "PrecioActividades");

            migrationBuilder.DropIndex(
                name: "IX_PrecioTraslados_ContratoId",
                table: "PrecioTraslados");

            migrationBuilder.DropIndex(
                name: "IX_PrecioRentaAutos_ContratoId",
                table: "PrecioRentaAutos");

            migrationBuilder.DropIndex(
                name: "IX_PrecioPlanesAlimenticios_ContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropIndex(
                name: "IX_PrecioAlojamiento_ContratoId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "PrecioTraslados");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "PrecioRentaAutos");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "PrecioPlanesAlimenticios");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "PrecioAlojamiento");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "RestriccionesPrecios",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "PrecioTraslados",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "PrecioPlanesAlimenticios",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "PrecioComodidades",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "PrecioAlojamiento",
                nullable: false,
                oldClrType: typeof(double));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "PrecioComodidades");

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "RestriccionesPrecios",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "PrecioTraslados",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "PrecioTraslados",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "PrecioRentaAutos",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "PrecioPlanesAlimenticios",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "PrecioPlanesAlimenticios",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "PrecioAlojamiento",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "PrecioAlojamiento",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrecioActividades",
                columns: table => new
                {
                    PrecioActividadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContratoId = table.Column<int>(nullable: true),
                    HorasAdicionales = table.Column<int>(nullable: false),
                    Incluido = table.Column<double>(nullable: false),
                    PrecioAdulto = table.Column<double>(nullable: false),
                    PrecioInfante = table.Column<double>(nullable: false),
                    PrecioNino = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecioActividades", x => x.PrecioActividadId);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrecioActividades_Temporadas_TemporadaId",
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
                name: "IX_PrecioRentaAutos_ContratoId",
                table: "PrecioRentaAutos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioPlanesAlimenticios_ContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_ContratoId",
                table: "PrecioAlojamiento",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_ContratoId",
                table: "PrecioActividades",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_ProductoId",
                table: "PrecioActividades",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrecioActividades_TemporadaId",
                table: "PrecioActividades",
                column: "TemporadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioAlojamiento_Contratos_ContratoId",
                table: "PrecioAlojamiento",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioPlanesAlimenticios_Contratos_ContratoId",
                table: "PrecioPlanesAlimenticios",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioRentaAutos_Contratos_ContratoId",
                table: "PrecioRentaAutos",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioTraslados_Contratos_ContratoId",
                table: "PrecioTraslados",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
