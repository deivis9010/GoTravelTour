using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoproductoalojamientoymodifcador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Productos_AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Modificadores_ModificadorId",
                table: "ModificadorProductos");

            migrationBuilder.DropIndex(
                name: "IX_ModificadorProductos_AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.DropColumn(
                name: "AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.AlterColumn<int>(
                name: "ModificadorId",
                table: "ModificadorProductos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Modificadores_ModificadorId",
                table: "ModificadorProductos",
                column: "ModificadorId",
                principalTable: "Modificadores",
                principalColumn: "ModificadorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Modificadores_ModificadorId",
                table: "ModificadorProductos");

            migrationBuilder.AlterColumn<int>(
                name: "ModificadorId",
                table: "ModificadorProductos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AlojamientoProductoId",
                table: "ModificadorProductos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorProductos_AlojamientoProductoId",
                table: "ModificadorProductos",
                column: "AlojamientoProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Productos_AlojamientoProductoId",
                table: "ModificadorProductos",
                column: "AlojamientoProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Modificadores_ModificadorId",
                table: "ModificadorProductos",
                column: "ModificadorId",
                principalTable: "Modificadores",
                principalColumn: "ModificadorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
