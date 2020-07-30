using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class agregandovoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_ConfiguracionVoucher_ConfiguracionVoucherId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_ConfiguracionVoucher_ConfiguracionVoucherId",
                table: "OrdenTraslado");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionVoucherId",
                table: "OrdenTraslado",
                newName: "VoucherConfiguracionVoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_ConfiguracionVoucherId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_VoucherConfiguracionVoucherId");

            migrationBuilder.RenameColumn(
                name: "ConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                newName: "VoucherConfiguracionVoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenAlojamiento_ConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                newName: "IX_OrdenAlojamiento_VoucherConfiguracionVoucherId");

            migrationBuilder.AddColumn<string>(
                name: "InformacionLlegada",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InformacionSalida",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenVehiculo_VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo",
                column: "VoucherConfiguracionVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                column: "VoucherConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenTraslado",
                column: "VoucherConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenVehiculo_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo",
                column: "VoucherConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenAlojamiento_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenAlojamiento");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenTraslado_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenTraslado");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenVehiculo_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo");

            migrationBuilder.DropIndex(
                name: "IX_OrdenVehiculo_VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "InformacionLlegada",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "InformacionSalida",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenVehiculo");

            migrationBuilder.RenameColumn(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenTraslado",
                newName: "ConfiguracionVoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenTraslado_VoucherConfiguracionVoucherId",
                table: "OrdenTraslado",
                newName: "IX_OrdenTraslado_ConfiguracionVoucherId");

            migrationBuilder.RenameColumn(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                newName: "ConfiguracionVoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_OrdenAlojamiento_VoucherConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                newName: "IX_OrdenAlojamiento_ConfiguracionVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenAlojamiento_ConfiguracionVoucher_ConfiguracionVoucherId",
                table: "OrdenAlojamiento",
                column: "ConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenTraslado_ConfiguracionVoucher_ConfiguracionVoucherId",
                table: "OrdenTraslado",
                column: "ConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
