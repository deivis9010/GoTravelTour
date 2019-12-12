using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addAlojActVehiculos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comodidades_CategoriaComodidad_CategoriaComodidadId",
                table: "Comodidades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriaComodidad",
                table: "CategoriaComodidad");

            migrationBuilder.RenameTable(
                name: "CategoriaComodidad",
                newName: "CategoriaComodidades");

            migrationBuilder.AddColumn<int>(
                name: "AlojamientoId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoAlojamientoId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Productos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriaComodidades",
                table: "CategoriaComodidades",
                column: "CategoriaComodidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoAlojamientoId",
                table: "Productos",
                column: "TipoAlojamientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comodidades_CategoriaComodidades_CategoriaComodidadId",
                table: "Comodidades",
                column: "CategoriaComodidadId",
                principalTable: "CategoriaComodidades",
                principalColumn: "CategoriaComodidadId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_TipoAlojamientos_TipoAlojamientoId",
                table: "Productos",
                column: "TipoAlojamientoId",
                principalTable: "TipoAlojamientos",
                principalColumn: "TipoAlojamientoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comodidades_CategoriaComodidades_CategoriaComodidadId",
                table: "Comodidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_TipoAlojamientos_TipoAlojamientoId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_TipoAlojamientoId",
                table: "Productos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriaComodidades",
                table: "CategoriaComodidades");

            migrationBuilder.DropColumn(
                name: "AlojamientoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoAlojamientoId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Productos");

            migrationBuilder.RenameTable(
                name: "CategoriaComodidades",
                newName: "CategoriaComodidad");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriaComodidad",
                table: "CategoriaComodidad",
                column: "CategoriaComodidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comodidades_CategoriaComodidad_CategoriaComodidadId",
                table: "Comodidades",
                column: "CategoriaComodidadId",
                principalTable: "CategoriaComodidad",
                principalColumn: "CategoriaComodidadId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
