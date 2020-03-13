using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoalojamentoproducto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlojamientosPlanesAlimenticios_Productos_AlojamientoId",
                table: "AlojamientosPlanesAlimenticios");

            migrationBuilder.RenameColumn(
                name: "AlojamientoId",
                table: "AlojamientosPlanesAlimenticios",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_AlojamientosPlanesAlimenticios_AlojamientoId",
                table: "AlojamientosPlanesAlimenticios",
                newName: "IX_AlojamientosPlanesAlimenticios_ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlojamientosPlanesAlimenticios_Productos_ProductoId",
                table: "AlojamientosPlanesAlimenticios",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlojamientosPlanesAlimenticios_Productos_ProductoId",
                table: "AlojamientosPlanesAlimenticios");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "AlojamientosPlanesAlimenticios",
                newName: "AlojamientoId");

            migrationBuilder.RenameIndex(
                name: "IX_AlojamientosPlanesAlimenticios_ProductoId",
                table: "AlojamientosPlanesAlimenticios",
                newName: "IX_AlojamientosPlanesAlimenticios_AlojamientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlojamientosPlanesAlimenticios_Productos_AlojamientoId",
                table: "AlojamientosPlanesAlimenticios",
                column: "AlojamientoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
