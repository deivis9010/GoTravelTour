using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class quitandoCampo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicio_Productos_ActividadProductoId",
                table: "Servicio");

            migrationBuilder.DropIndex(
                name: "IX_Servicio_ActividadProductoId",
                table: "Servicio");

            migrationBuilder.DropColumn(
                name: "ActividadProductoId",
                table: "Servicio");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActividadProductoId",
                table: "Servicio",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicio_ActividadProductoId",
                table: "Servicio",
                column: "ActividadProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicio_Productos_ActividadProductoId",
                table: "Servicio",
                column: "ActividadProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
