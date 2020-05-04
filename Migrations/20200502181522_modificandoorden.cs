using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modificandoorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_Orden_OrdenId",
                table: "OrdenActividad");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_Orden_OrdenId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_Orden_OrdenId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenVehiculo_Orden_OrdenId",
                table: "OrdenVehiculo");

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenVehiculo",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenTraslado",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenAlojamiento",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenActividad",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_Orden_OrdenId",
                table: "OrdenActividad",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_Orden_OrdenId",
                table: "OrdenAlojamiento",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_Orden_OrdenId",
                table: "OrdenTraslado",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenVehiculo_Orden_OrdenId",
                table: "OrdenVehiculo",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_Orden_OrdenId",
                table: "OrdenActividad");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_Orden_OrdenId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_Orden_OrdenId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenVehiculo_Orden_OrdenId",
                table: "OrdenVehiculo");

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenVehiculo",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenTraslado",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenAlojamiento",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "OrdenId",
                table: "OrdenActividad",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_Orden_OrdenId",
                table: "OrdenActividad",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_Orden_OrdenId",
                table: "OrdenAlojamiento",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_Orden_OrdenId",
                table: "OrdenTraslado",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenVehiculo_Orden_OrdenId",
                table: "OrdenVehiculo",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
