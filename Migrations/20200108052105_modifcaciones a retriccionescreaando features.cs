using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifcacionesaretriccionescreaandofeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesActividades");

            migrationBuilder.DropTable(
                name: "RestriccionesRentasAutos");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alojamiento_Latitud",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alojamiento_Longitud",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Alojamiento_PermiteAdult",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Alojamiento_PermiteInfante",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Alojamiento_PermiteNino",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Restricciones",
                columns: table => new
                {
                    RestriccionesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Minimo = table.Column<int>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    IsActivo = table.Column<bool>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restricciones", x => x.RestriccionesId);
                    table.ForeignKey(
                        name: "FK_Restricciones_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servicio",
                columns: table => new
                {
                    ServicioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Opcional = table.Column<bool>(nullable: false),
                    Categoria = table.Column<string>(nullable: true),
                    ProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicio", x => x.ServicioId);
                    table.ForeignKey(
                        name: "FK_Servicio_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesPrecios",
                columns: table => new
                {
                    RestriccionesPrecioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Precio = table.Column<double>(nullable: false),
                    RestriccionesId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesPrecios", x => x.RestriccionesPrecioId);
                    table.ForeignKey(
                        name: "FK_RestriccionesPrecios_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionesPrecios_Restricciones_RestriccionesId",
                        column: x => x.RestriccionesId,
                        principalTable: "Restricciones",
                        principalColumn: "RestriccionesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_RegionId",
                table: "Productos",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Restricciones_TemporadaId",
                table: "Restricciones",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesPrecios_ProductoId",
                table: "RestriccionesPrecios",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesPrecios_RestriccionesId",
                table: "RestriccionesPrecios",
                column: "RestriccionesId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_ProductoId",
                table: "Servicio",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos",
                column: "RegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "RestriccionesPrecios");

            migrationBuilder.DropTable(
                name: "Servicio");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropIndex(
                name: "IX_Productos_RegionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_Latitud",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_Longitud",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_PermiteAdult",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_PermiteInfante",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Alojamiento_PermiteNino",
                table: "Productos");

            migrationBuilder.CreateTable(
                name: "RestriccionesActividades",
                columns: table => new
                {
                    RestriccionesActividadId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContratoId = table.Column<int>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    Minimo = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesActividades", x => x.RestriccionesActividadId);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionesActividades_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesRentasAutos",
                columns: table => new
                {
                    RestriccionesRentasAutosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContratoId = table.Column<int>(nullable: true),
                    IsActivo = table.Column<bool>(nullable: false),
                    Maximo = table.Column<int>(nullable: false),
                    Minimo = table.Column<int>(nullable: false),
                    Precio = table.Column<double>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    TemporadaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesRentasAutos", x => x.RestriccionesRentasAutosId);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionesRentasAutos_Temporadas_TemporadaId",
                        column: x => x.TemporadaId,
                        principalTable: "Temporadas",
                        principalColumn: "TemporadaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_ContratoId",
                table: "RestriccionesActividades",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_ProductoId",
                table: "RestriccionesActividades",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesActividades_TemporadaId",
                table: "RestriccionesActividades",
                column: "TemporadaId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_ContratoId",
                table: "RestriccionesRentasAutos",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_ProductoId",
                table: "RestriccionesRentasAutos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesRentasAutos_TemporadaId",
                table: "RestriccionesRentasAutos",
                column: "TemporadaId");
        }
    }
}
