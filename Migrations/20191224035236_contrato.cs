using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class contrato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "Productos");

            migrationBuilder.AddColumn<int>(
                name: "MarcaId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModeloId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "NombreTemporadas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "AlmacenImagenes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContratoProducto",
                columns: table => new
                {
                    ContratoProductoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContratoId = table.Column<int>(nullable: true),
                    ProductoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoProducto", x => x.ContratoProductoId);
                    table.ForeignKey(
                        name: "FK_ContratoProducto_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContratoProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_MarcaId",
                table: "Productos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ModeloId",
                table: "Productos",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_NombreTemporadas_ContratoId",
                table: "NombreTemporadas",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenImagenes_ProductoId",
                table: "AlmacenImagenes",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoProducto_ContratoId",
                table: "ContratoProducto",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoProducto_ProductoId",
                table: "ContratoProducto",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlmacenImagenes_Productos_ProductoId",
                table: "AlmacenImagenes",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NombreTemporadas_Contratos_ContratoId",
                table: "NombreTemporadas",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "MarcaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Modelos_ModeloId",
                table: "Productos",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "ModeloId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlmacenImagenes_Productos_ProductoId",
                table: "AlmacenImagenes");

            migrationBuilder.DropForeignKey(
                name: "FK_NombreTemporadas_Contratos_ContratoId",
                table: "NombreTemporadas");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Modelos_ModeloId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "ContratoProducto");

            migrationBuilder.DropIndex(
                name: "IX_Productos_MarcaId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ModeloId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_NombreTemporadas_ContratoId",
                table: "NombreTemporadas");

            migrationBuilder.DropIndex(
                name: "IX_AlmacenImagenes_ProductoId",
                table: "AlmacenImagenes");

            migrationBuilder.DropColumn(
                name: "MarcaId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ModeloId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "NombreTemporadas");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "AlmacenImagenes");

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "Productos",
                nullable: true);
        }
    }
}
