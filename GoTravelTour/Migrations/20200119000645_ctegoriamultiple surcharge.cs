using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class ctegoriamultiplesurcharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clase",
                table: "Productos");

            migrationBuilder.CreateTable(
                name: "CategoriaAuto",
                columns: table => new
                {
                    CategoriaAutoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaAuto", x => x.CategoriaAutoId);
                });

            migrationBuilder.CreateTable(
                name: "Sobreprecio",
                columns: table => new
                {
                    SobreprecioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PrecioDesde = table.Column<decimal>(nullable: false),
                    PrecioHasta = table.Column<decimal>(nullable: false),
                    ValorPorCiento = table.Column<decimal>(nullable: false),
                    ValorDinero = table.Column<decimal>(nullable: false),
                    PagoPorDia = table.Column<bool>(nullable: false),
                    TipoProductoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sobreprecio", x => x.SobreprecioId);
                    table.ForeignKey(
                        name: "FK_Sobreprecio_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoCategoriaAuto",
                columns: table => new
                {
                    VehiculoCategoriaAutoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductoId = table.Column<int>(nullable: false),
                    CategoriaAutoId = table.Column<int>(nullable: false),
                    VehiculoProductoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoCategoriaAuto", x => x.VehiculoCategoriaAutoId);
                    table.ForeignKey(
                        name: "FK_VehiculoCategoriaAuto_CategoriaAuto_CategoriaAutoId",
                        column: x => x.CategoriaAutoId,
                        principalTable: "CategoriaAuto",
                        principalColumn: "CategoriaAutoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehiculoCategoriaAuto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehiculoCategoriaAuto_Productos_VehiculoProductoId",
                        column: x => x.VehiculoProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sobreprecio_TipoProductoId",
                table: "Sobreprecio",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoCategoriaAuto_CategoriaAutoId",
                table: "VehiculoCategoriaAuto",
                column: "CategoriaAutoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoCategoriaAuto_ProductoId",
                table: "VehiculoCategoriaAuto",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoCategoriaAuto_VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                column: "VehiculoProductoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sobreprecio");

            migrationBuilder.DropTable(
                name: "VehiculoCategoriaAuto");

            migrationBuilder.DropTable(
                name: "CategoriaAuto");

            migrationBuilder.AddColumn<string>(
                name: "Clase",
                table: "Productos",
                nullable: true);
        }
    }
}
