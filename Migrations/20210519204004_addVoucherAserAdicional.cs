using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addVoucherAserAdicional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroConfirmacion",
                table: "OrdenServicioAdicional",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional",
                column: "VoucherConfiguracionVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenServicioAdicional_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional",
                column: "VoucherConfiguracionVoucherId",
                principalTable: "ConfiguracionVoucher",
                principalColumn: "ConfiguracionVoucherId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenServicioAdicional_ConfiguracionVoucher_VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropIndex(
                name: "IX_OrdenServicioAdicional_VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "NumeroConfirmacion",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "VoucherConfiguracionVoucherId",
                table: "OrdenServicioAdicional");
        }
    }
}
