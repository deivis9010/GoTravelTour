using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiosalojamientp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_PrecioAlojamiento_PrecioAlojamientoId",
                table: "Modificadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_Temporadas_TemporadaId",
                table: "Modificadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_TipoProductos_TipoProductoId",
                table: "Modificadores");

            migrationBuilder.DropIndex(
                name: "IX_Modificadores_PrecioAlojamientoId",
                table: "Modificadores");

            migrationBuilder.DropIndex(
                name: "IX_Modificadores_TemporadaId",
                table: "Modificadores");

            migrationBuilder.DropColumn(
                name: "PrecioAlojamientoId",
                table: "Modificadores");

            migrationBuilder.DropColumn(
                name: "TemporadaId",
                table: "Modificadores");

            migrationBuilder.RenameColumn(
                name: "TipoProductoId",
                table: "Modificadores",
                newName: "ProveedorId");

            migrationBuilder.RenameIndex(
                name: "IX_Modificadores_TipoProductoId",
                table: "Modificadores",
                newName: "IX_Modificadores_ProveedorId");

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "PrecioAlojamiento",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlojamientoProductoId",
                table: "ModificadorProductos",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContratoId",
                table: "Modificadores",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_PrecioAlojamiento_ContratoId",
                table: "PrecioAlojamiento",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ModificadorProductos_AlojamientoProductoId",
                table: "ModificadorProductos",
                column: "AlojamientoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_ContratoId",
                table: "Modificadores",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_Contratos_ContratoId",
                table: "Modificadores",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_Proveedores_ProveedorId",
                table: "Modificadores",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "ProveedorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModificadorProductos_Productos_AlojamientoProductoId",
                table: "ModificadorProductos",
                column: "AlojamientoProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrecioAlojamiento_Contratos_ContratoId",
                table: "PrecioAlojamiento",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "ContratoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_Contratos_ContratoId",
                table: "Modificadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Modificadores_Proveedores_ProveedorId",
                table: "Modificadores");

            migrationBuilder.DropForeignKey(
                name: "FK_ModificadorProductos_Productos_AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_PrecioAlojamiento_Contratos_ContratoId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_PrecioAlojamiento_ContratoId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropIndex(
                name: "IX_ModificadorProductos_AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.DropIndex(
                name: "IX_Modificadores_ContratoId",
                table: "Modificadores");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "PrecioAlojamiento");

            migrationBuilder.DropColumn(
                name: "AlojamientoProductoId",
                table: "ModificadorProductos");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                table: "Modificadores",
                newName: "TipoProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_Modificadores_ProveedorId",
                table: "Modificadores",
                newName: "IX_Modificadores_TipoProductoId");

            migrationBuilder.AlterColumn<int>(
                name: "ContratoId",
                table: "Modificadores",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrecioAlojamientoId",
                table: "Modificadores",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemporadaId",
                table: "Modificadores",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_PrecioAlojamientoId",
                table: "Modificadores",
                column: "PrecioAlojamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modificadores_TemporadaId",
                table: "Modificadores",
                column: "TemporadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_PrecioAlojamiento_PrecioAlojamientoId",
                table: "Modificadores",
                column: "PrecioAlojamientoId",
                principalTable: "PrecioAlojamiento",
                principalColumn: "PrecioAlojamientoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_Temporadas_TemporadaId",
                table: "Modificadores",
                column: "TemporadaId",
                principalTable: "Temporadas",
                principalColumn: "TemporadaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modificadores_TipoProductos_TipoProductoId",
                table: "Modificadores",
                column: "TipoProductoId",
                principalTable: "TipoProductos",
                principalColumn: "TipoProductoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
