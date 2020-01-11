using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class dd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comodidades_Productos_ActividadProductoId",
                table: "Comodidades");

            migrationBuilder.DropIndex(
                name: "IX_Comodidades_ActividadProductoId",
                table: "Comodidades");

            migrationBuilder.DropColumn(
                name: "ActividadProductoId",
                table: "Comodidades");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ActividadProductoId",
                table: "Comodidades",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comodidades_ActividadProductoId",
                table: "Comodidades",
                column: "ActividadProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comodidades_Productos_ActividadProductoId",
                table: "Comodidades",
                column: "ActividadProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
