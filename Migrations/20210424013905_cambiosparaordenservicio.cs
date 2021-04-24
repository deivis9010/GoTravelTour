using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class cambiosparaordenservicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistribuidorId",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdBillQB",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioOrden",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_DistribuidorId",
                table: "OrdenServicioAdicional",
                column: "DistribuidorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenServicioAdicional_Distribuidores_DistribuidorId",
                table: "OrdenServicioAdicional",
                column: "DistribuidorId",
                principalTable: "Distribuidores",
                principalColumn: "DistribuidorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenServicioAdicional_Distribuidores_DistribuidorId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropIndex(
                name: "IX_OrdenServicioAdicional_DistribuidorId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "DistribuidorId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "IdBillQB",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "PrecioOrden",
                table: "OrdenServicioAdicional");
        }
    }
}
