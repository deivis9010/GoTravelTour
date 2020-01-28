using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class quitandoreferencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiculoCategoriaAuto_Productos_VehiculoProductoId",
                table: "VehiculoCategoriaAuto");

            migrationBuilder.DropIndex(
                name: "IX_VehiculoCategoriaAuto_VehiculoProductoId",
                table: "VehiculoCategoriaAuto");

            migrationBuilder.DropColumn(
                name: "VehiculoProductoId",
                table: "VehiculoCategoriaAuto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoCategoriaAuto_VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                column: "VehiculoProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiculoCategoriaAuto_Productos_VehiculoProductoId",
                table: "VehiculoCategoriaAuto",
                column: "VehiculoProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
