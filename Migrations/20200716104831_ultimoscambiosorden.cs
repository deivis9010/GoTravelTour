using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class ultimoscambiosorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DireccionReferencia",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoReferencia",
                table: "OrdenVehiculo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionReferencia",
                table: "OrdenTraslado",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoReferencia",
                table: "OrdenTraslado",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenActividad",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenActividad_VoucherConfiguracionVoucherId",
                table: "OrdenActividad",
                column: "VoucherConfiguracionVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenActividad_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenActividad",
                column: "VoucherConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenActividad_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenActividad");

            migrationBuilder.DropIndex(
                name: "IX_OrdenActividad_VoucherConfiguracionVoucherId",
                table: "OrdenActividad");

            migrationBuilder.DropColumn(
                name: "DireccionReferencia",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "TelefonoReferencia",
                table: "OrdenVehiculo");

            migrationBuilder.DropColumn(
                name: "DireccionReferencia",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "TelefonoReferencia",
                table: "OrdenTraslado");

            migrationBuilder.DropColumn(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenActividad");
        }
    }
}
