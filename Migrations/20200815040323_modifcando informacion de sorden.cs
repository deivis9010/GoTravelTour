using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class modifcandoinformaciondesorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenciasOFACId",
                table: "DatosPasajeroSecundario",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LicenciasOFACId",
                table: "DatosPasajeroPrimario",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatosPasajeroSecundario_LicenciasOFACId",
                table: "DatosPasajeroSecundario",
                column: "LicenciasOFACId");

            migrationBuilder.CreateIndex(
                name: "IX_DatosPasajeroPrimario_LicenciasOFACId",
                table: "DatosPasajeroPrimario",
                column: "LicenciasOFACId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatosPasajeroPrimario_LicenciasOFAC_LicenciasOFACId",
                table: "DatosPasajeroPrimario",
                column: "LicenciasOFACId",
                principalTable: "LicenciasOFAC",
                principalColumn: "LicenciasOFACId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DatosPasajeroSecundario_LicenciasOFAC_LicenciasOFACId",
                table: "DatosPasajeroSecundario",
                column: "LicenciasOFACId",
                principalTable: "LicenciasOFAC",
                principalColumn: "LicenciasOFACId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatosPasajeroPrimario_LicenciasOFAC_LicenciasOFACId",
                table: "DatosPasajeroPrimario");

            migrationBuilder.DropForeignKey(
                name: "FK_DatosPasajeroSecundario_LicenciasOFAC_LicenciasOFACId",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropIndex(
                name: "IX_DatosPasajeroSecundario_LicenciasOFACId",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropIndex(
                name: "IX_DatosPasajeroPrimario_LicenciasOFACId",
                table: "DatosPasajeroPrimario");

            migrationBuilder.DropColumn(
                name: "LicenciasOFACId",
                table: "DatosPasajeroSecundario");

            migrationBuilder.DropColumn(
                name: "LicenciasOFACId",
                table: "DatosPasajeroPrimario");
        }
    }
}
