using Microsoft.EntityFrameworkCore.Migrations;

namespace GoTravelTour.Migrations
{
    public partial class addOrdenServicio3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdenId",
                table: "OrdenServicioAdicional",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServicioAdicional_OrdenId",
                table: "OrdenServicioAdicional",
                column: "OrdenId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenServicioAdicional_Orden_OrdenId",
                table: "OrdenServicioAdicional",
                column: "OrdenId",
                principalTable: "Orden",
                principalColumn: "OrdenId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenServicioAdicional_Orden_OrdenId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropIndex(
                name: "IX_OrdenServicioAdicional_OrdenId",
                table: "OrdenServicioAdicional");

            migrationBuilder.DropColumn(
                name: "OrdenId",
                table: "OrdenServicioAdicional");
        }
    }
}
