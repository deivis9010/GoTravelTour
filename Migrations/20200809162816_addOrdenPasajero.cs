using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addOrdenPasajero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdenId",
                table: "DatosPasajeroPrimario",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatosPasajeroPrimario_OrdenId",
                table: "DatosPasajeroPrimario",
                column: "OrdenId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatosPasajeroPrimario_Orden_OrdenId",
                table: "DatosPasajeroPrimario",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatosPasajeroPrimario_Orden_OrdenId",
                table: "DatosPasajeroPrimario");

            migrationBuilder.DropIndex(
                name: "IX_DatosPasajeroPrimario_OrdenId",
                table: "DatosPasajeroPrimario");

            migrationBuilder.DropColumn(
                name: "OrdenId",
                table: "DatosPasajeroPrimario");
        }
    }
}
