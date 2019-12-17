using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addllavesrutas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_PuntosInteres_Regiones_RegionId",
                table: "PuntosInteres");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Regiones_regionDestinoRegionId",
                table: "Rutas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutas_Regiones_regionOrigenRegionId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_regionDestinoRegionId",
                table: "Rutas");

            migrationBuilder.DropIndex(
                name: "IX_Rutas_regionOrigenRegionId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "regionDestinoRegionId",
                table: "Rutas");

            migrationBuilder.DropColumn(
                name: "regionOrigenRegionId",
                table: "Rutas");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "PuntosInteres",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PuntoInteresId",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos",
                column: "RegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PuntosInteres_Regiones_RegionId",
                table: "PuntosInteres",
                column: "RegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_PuntosInteres_Regiones_RegionId",
                table: "PuntosInteres");

            migrationBuilder.AddColumn<int>(
                name: "regionDestinoRegionId",
                table: "Rutas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "regionOrigenRegionId",
                table: "Rutas",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "PuntosInteres",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Productos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PuntoInteresId",
                table: "Productos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_regionDestinoRegionId",
                table: "Rutas",
                column: "regionDestinoRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutas_regionOrigenRegionId",
                table: "Rutas",
                column: "regionOrigenRegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId",
                principalTable: "PuntosInteres",
                principalColumn: "PuntoInteresId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos",
                column: "RegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PuntosInteres_Regiones_RegionId",
                table: "PuntosInteres",
                column: "RegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Regiones_regionDestinoRegionId",
                table: "Rutas",
                column: "regionDestinoRegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutas_Regiones_regionOrigenRegionId",
                table: "Rutas",
                column: "regionOrigenRegionId",
                principalTable: "Regiones",
                principalColumn: "RegionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
