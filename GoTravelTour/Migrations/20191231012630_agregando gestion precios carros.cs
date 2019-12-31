using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandogestionprecioscarros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratoProducto");

            migrationBuilder.AddColumn<int>(
                name: "DistribuidorId",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_DistribuidorId",
                table: "Contratos",
                column: "DistribuidorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Distribuidores_DistribuidorId",
                table: "Contratos",
                column: "DistribuidorId",
                principalTable: "Distribuidores",
                principalColumn: "DistribuidorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Distribuidores_DistribuidorId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_DistribuidorId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "DistribuidorId",
                table: "Contratos");

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
                name: "IX_ContratoProducto_ContratoId",
                table: "ContratoProducto",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoProducto_ProductoId",
                table: "ContratoProducto",
                column: "ProductoId");
        }
    }
}
