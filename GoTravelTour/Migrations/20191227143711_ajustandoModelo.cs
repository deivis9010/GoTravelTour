using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class ajustandoModelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_PuntosInteres_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Regiones_RegionId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_RegionId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PuntoInteresId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Productos");

            migrationBuilder.AddColumn<bool>(
                name: "PermiteHacerCopia",
                table: "Temporadas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivo",
                table: "Proveedores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteNino",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteInfante",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteAdult",
                table: "Productos",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "PermiteHacerCopia",
                table: "Modificadores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteHacerCopia",
                table: "Habitaciones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivo",
                table: "Distribuidores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteHacerCopia",
                table: "Contratos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermiteHacerCopia",
                table: "Temporadas");

            migrationBuilder.DropColumn(
                name: "IsActivo",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "PermiteHacerCopia",
                table: "Modificadores");

            migrationBuilder.DropColumn(
                name: "PermiteHacerCopia",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "IsActivo",
                table: "Distribuidores");

            migrationBuilder.DropColumn(
                name: "PermiteHacerCopia",
                table: "Contratos");

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteNino",
                table: "Productos",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteInfante",
                table: "Productos",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PermiteAdult",
                table: "Productos",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuntoInteresId",
                table: "Productos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Productos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_PuntoInteresId",
                table: "Productos",
                column: "PuntoInteresId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_RegionId",
                table: "Productos",
                column: "RegionId");

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
        }
    }
}
