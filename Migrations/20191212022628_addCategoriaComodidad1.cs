using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addCategoriaComodidad1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActividadId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadPersonas",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duracion",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasTransporte",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxDuracion",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modalidad",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadPlazas",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoTransmision",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehiculoId",
                table: "Productos",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comodidades_Productos_ActividadProductoId",
                table: "Comodidades");

            migrationBuilder.DropIndex(
                name: "IX_Comodidades_ActividadProductoId",
                table: "Comodidades");

            migrationBuilder.DropColumn(
                name: "ActividadId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CantidadPersonas",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "HasTransporte",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "MaxDuracion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Modalidad",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CantidadPlazas",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoTransmision",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "VehiculoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ActividadProductoId",
                table: "Comodidades");
        }
    }
}
