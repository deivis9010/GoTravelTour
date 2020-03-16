using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandomodificadorproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Productos_ProductoId",
                table: "ModificadorProductos");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "ModificadorProductos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Productos_ProductoId",
                table: "ModificadorProductos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Productos_ProductoId",
                table: "ModificadorProductos");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                table: "ModificadorProductos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Productos_ProductoId",
                table: "ModificadorProductos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
