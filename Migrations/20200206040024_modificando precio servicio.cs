using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoprecioservicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioServicio_Productos_ProductoId",
                table: "PrecioServicio");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "PrecioServicio",
                newName: "ServicioId");

            migrationBuilder.RenameIndex(
                name: "IX_PrecioServicio_ProductoId",
                table: "PrecioServicio",
                newName: "IX_PrecioServicio_ServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioServicio_Servicio_ServicioId",
                table: "PrecioServicio",
                column: "ServicioId",
                principalTable: "Servicio",
                principalColumn: "ServicioId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrecioServicio_Servicio_ServicioId",
                table: "PrecioServicio");

            migrationBuilder.RenameColumn(
                name: "ServicioId",
                table: "PrecioServicio",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_PrecioServicio_ServicioId",
                table: "PrecioServicio",
                newName: "IX_PrecioServicio_ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioServicio_Productos_ProductoId",
                table: "PrecioServicio",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
